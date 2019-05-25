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
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.PartnerCenter;
    using Microsoft.Extensions.Logging;
    using Microsoft.Store.PartnerCenter.Models.Auditing;
    using Microsoft.Store.PartnerCenter.Models.Customers;
    using Models;
    using Records;

    public static class Partners
    {
        [FunctionName("PartnerDeltaSync")]
        public static async Task PartnerDeltaSyncAsync(
            [QueueTrigger(
                "partnerdeltasync",
                Connection = "StorageConnectionString")]EnvironmentRecord input,
            [CosmosDB(
                databaseName: "smartoffice",
                collectionName: "customers",
                ConnectionStringSetting = "CosmosDbConnectionString")]DocumentClient client,
            [AuditRecord(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                ApplicationSecretName = "{PartnerCenterEndpoint.ApplicationSecretName}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                EndDate = "{AuditEndDate}",
                StartDate = "{AuditStartDate}")]List<AuditRecord> records,
            [Queue(
                "controlprofilesync", Connection = "StorageConnectionString")]IAsyncCollector<CustomerRecord> profileSync,
            [Queue(
                "securitysync", Connection = "StorageConnectionString")]IAsyncCollector<CustomerRecord> securitySync,
            ILogger log)
        {
            List<CustomerEntry> customers = await DocumentRepository.GetAsync<CustomerEntry>(client, "smartoffice", "environments").ConfigureAwait(false);

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
                await profileSync.AddAsync(GetCustomerRecord(entry, input.AppEndpoint)).ConfigureAwait(false);
                await securitySync.AddAsync(GetCustomerRecord(entry, input.AppEndpoint)).ConfigureAwait(false);
            });
        }

        [FunctionName("PartnerFullSync")]
        public static void PartnerFullSync(
            [QueueTrigger(
                "partnerfullsync",
                Connection = "StorageConnectionString")]EnvironmentRecord input,
            [Customer(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                ApplicationSecretName = "{PartnerCenterEndpoint.ApplicationSecretName}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%")]List<Customer> customers,
            [Queue(
                "controlprofilesync", Connection = "StorageConnectionString")]ICollector<CustomerRecord> profileSync,
            [Queue(
                "securitysync", Connection = "StorageConnectionString")]ICollector<CustomerRecord> securitySync,
            ILogger log)
        {
            log.LogInformation($"Processing {input.FriendlyName}");

            customers.ForEach((customer) =>
            {
                CustomerEntry entry = GetCustomerEntry(customer);

                profileSync.Add(GetCustomerRecord(entry, input.AppEndpoint));
                securitySync.Add(GetCustomerRecord(entry, input.AppEndpoint));
            });
        }

        private static CustomerEntry GetCustomerEntry(Customer customer)
        {
            CustomerEntry entry;

            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }

            entry = ResourceConverter.Convert<Customer, CustomerEntry>(customer);

            entry.LastProcessed = null;
            entry.Name = customer.CompanyProfile.CompanyName;

            return entry;
        }

        private static CustomerRecord GetCustomerRecord(CustomerEntry entry, EndpointEntry endpoint)
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

            record.AppEndpoint = endpoint;
            record.SecureScorePeriod = period;

            return record;
        }
    }
}