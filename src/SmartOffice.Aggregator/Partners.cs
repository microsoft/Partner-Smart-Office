// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Converters;
    using Data;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.AzureMonitor;
    using Microsoft.Azure.WebJobs.Extensions.PartnerCenter;
    using Microsoft.Extensions.Logging;
    using Microsoft.Store.PartnerCenter.Models.Auditing;
    using Microsoft.Store.PartnerCenter.Models.Customers;
    using Models;

    /// <summary>
    /// Defines the Azure Functions used to aggregate partner information.
    /// </summary>
    public static class Partners
    {
        [FunctionName(Constants.PartnerDeltaSync)]
        public static async Task PartnerDeltaSyncAsync(
            [QueueTrigger(Constants.PartnerDeltaSync, Connection = Constants.StorageConnectionString)]EnvironmentRecord input,
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: Constants.EnvironmentsCollection,
                ConnectionStringSetting = Constants.CosmosDbConnectionString)]DocumentClient client,
            [AuditRecord(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                ApplicationSecretName = "{PartnerCenterEndpoint.ApplicationSecretName}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                EndDate = "{AuditEndDate}",
                StartDate = "{AuditStartDate}")]List<AuditRecord> records,
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: Constants.EnvironmentsCollection,
                ConnectionStringSetting = Constants.CosmosDbConnectionString,
                CreateIfNotExists = true,
                PartitionKey = "/environmentId")]IAsyncCollector<CustomerEntry> output,
            [Queue(Constants.ControlProfileSync, Connection = Constants.StorageConnectionString)]IAsyncCollector<CustomerRecord> profileSync,
            [Queue(Constants.SecurtityEventSync, Connection = Constants.StorageConnectionString)]IAsyncCollector<CustomerRecord> securitySync,
            ILogger log)
        {
            List<CustomerEntry> customers = await DocumentRepository.QueryAsync<CustomerEntry>(
                client,
                Constants.DatabaseName,
                Constants.EnvironmentsCollection,
                "/environmentId",
                new SqlQuerySpec
                {
                    Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@entryType", nameof(CustomerEntry)),
                    },
                    QueryText = "SELECT * FROM c WHERE c.entryType = '@entryType'",
                },
                false).ConfigureAwait(false);

            // TODO 
            // Refactor that way you have to a way to track changes with the customers. That way you are only submitting the customers
            // that change (e.g. a customer has changed their name). This will make it where you are not always update a customer after
            // entry stored in Cosmos DB.

            records.Where(r => r.OperationStatus == OperationStatus.Succeeded && !string.IsNullOrEmpty(r.CustomerId))
                .OrderBy(r => r.OperationDate).ToList().ForEach((r) =>
                {
                    if (r.OperationType == OperationType.AddCustomer)
                    {
                        Customer resource = AuditRecordConverter.Convert<Customer>(r);
                        CustomerEntry entry = ResourceConverter.Convert<Customer, CustomerEntry>(resource);

                        customers.Add(entry);
                    }
                });

            customers.ForEach(async (entry) =>
            {
                log.LogInformation($"Processing {entry.Name} from the {entry.EnvironmentName} environment");

                await output.AddAsync(entry).ConfigureAwait(false);
                await profileSync.AddAsync(GetCustomerRecord(entry, input)).ConfigureAwait(false);
                await securitySync.AddAsync(GetCustomerRecord(entry, input)).ConfigureAwait(false);
            });

        }

        [FunctionName(Constants.PartnerFullSync)]
        public static async Task PartnerFullSyncAsync(
            [QueueTrigger(Constants.PartnerFullSync, Connection = Constants.StorageConnectionString)]EnvironmentRecord input,
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: Constants.EnvironmentsCollection,
                ConnectionStringSetting = Constants.CosmosDbConnectionString)]DocumentClient client,
            [Customer(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                ApplicationSecretName = "{PartnerCenterEndpoint.ApplicationSecretName}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%")]List<Customer> customers,
            [AuditRecord(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                ApplicationSecretName = "{PartnerCenterEndpoint.ApplicationSecretName}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                EndDate = "{AuditEndDate}",
                StartDate = "{AuditStartDate}")]List<AuditRecord> records,
            [DataCollector(
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                WorkspaceId = "{WorkspaceId}",
                WorkspaceKeyName = "{WorkspaceKeyName}")]IAsyncCollector<LogData> datacCollector,
            [Queue(Constants.ControlProfileSync, Connection = Constants.StorageConnectionString)]IAsyncCollector<CustomerRecord> profileSync,
            [Queue(Constants.SecurtityEventSync, Connection = Constants.StorageConnectionString)]IAsyncCollector<CustomerRecord> securitySync,
            ILogger log)
        {
            foreach (Customer customer in customers)
            {
                CustomerEntry entry = GetCustomerEntry(customer, input);

                log.LogInformation($"Processing {entry.Name} from the {entry.EnvironmentName} environment");

                await profileSync.AddAsync(GetCustomerRecord(entry, input)).ConfigureAwait(false);
                await securitySync.AddAsync(GetCustomerRecord(entry, input)).ConfigureAwait(false);
            }

            await DocumentRepository.AddOrUpdateAsync(
                client,
                Constants.DatabaseName,
                Constants.EnvironmentsCollection,
                input.Id,
                customers.Select(c => GetCustomerEntry(c, input))).ConfigureAwait(false);

            await DocumentRepository.AddOrUpdateAsync(
                client,
                Constants.DatabaseName,
                Constants.EnvironmentsCollection,
                input.Id,
                records.Select(r => new PartnerDataEntry<List<AuditRecord>>
                {
                    Entry = records,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = r.Id
                })).ConfigureAwait(false);

            await datacCollector.AddAsync(new LogData
            {
                Data = records,
                LogType = "PartnerCenterAudit",
                TimeStampField = "OperationDate"
            }).ConfigureAwait(false);
        }

        private static CustomerEntry GetCustomerEntry(Customer customer, EnvironmentRecord environment)
        {
            CustomerEntry entry;

            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            entry = ResourceConverter.Convert<Customer, CustomerEntry>(customer);

            entry.EntryType = nameof(CustomerEntry);
            entry.EnvironmentId = environment.EnvironmentId;
            entry.EnvironmentName = environment.EnvironmentName;
            entry.LastProcessed = null;
            entry.Name = customer.CompanyProfile.CompanyName;

            return entry;
        }

        private static CustomerRecord GetCustomerRecord(CustomerEntry entry, EnvironmentRecord environment)
        {
            CustomerRecord record;
            int period;

            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            record = ResourceConverter.Convert<CustomerEntry, CustomerRecord>(entry);

            // Calculate the period, the number of days of Secure Score results to retrieve starting from current date, based on the last porcessed time.
            period = (entry.LastProcessed == null) ? 30 : (DateTimeOffset.UtcNow - entry.LastProcessed).Value.Days;

            // Ensure the period is between 1 and 30. You cannot request more than the past 30 days of Secure results.
            if (period < 1)
            {
                period = 1;
            }
            else if (period > 30)
            {
                period = 30;
            }

            record.AppEndpoint = environment.AppEndpoint;
            record.EnvironmentId = environment.Id;
            record.EnvironmentName = environment.EnvironmentName;
            record.SecureScorePeriod = period;
            record.WorkspaceId = environment.WorkspaceId;
            record.WorkspaceKeyName = environment.WorkspaceKeyName;

            return record;
        }
    }
}