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
                ApplicationId = "{PartnerCenter.ApplicationId}",
                ApplicationSecretName = "{PartnerCenter.ApplicationSecret}",
                ApplicationTenantId = "{PartnerCenter.TenantId}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                EndDate = "{AuditEndDate}",
                StartDate = "{AuditStartDate}")]List<AuditRecord> records,
            [CosmosDB(
                databaseName: "smartoffice",
                collectionName: "customers",
                ConnectionStringSetting = "CosmosDbConnectionString",
                CreateIfNotExists = true)]IAsyncCollector<CustomerEntry> items,
            [Queue(
                "securitysync", Connection = "StorageConnectionString")] ICollector<CustomerRecord> output,
            ILogger log)
        {
            List<CustomerEntry> customers = await DocumentRepository.GetAsync<CustomerEntry>(client, "smartoffice", "environments").ConfigureAwait(false);

            records.Where(r => r.OperationStatus == OperationStatus.Succeeded && !string.IsNullOrEmpty(r.CustomerId))
                .OrderBy(r => r.OperationDate).ToList().ForEach(async (r) =>
            {
                if (r.OperationType == OperationType.AddCustomer)
                {
                    Customer resource = AuditRecordConverter.Convert<Customer>(r);
                    CustomerEntry entry = ResourceConverter.Convert<Customer, CustomerEntry>(resource);

                    customers.Add(entry);
                    await items.AddAsync(entry).ConfigureAwait(false);
                }
            });

            customers.ForEach((entry) =>
            {
                output.Add(GetCustomerRecord(entry, input.AppEndpoint));
            });
        }

        [FunctionName("PartnerFullSync")]
        public static void PartnerFullSync(
            [QueueTrigger(
                "partnerfullsync",
                Connection = "StorageConnectionString")]EnvironmentRecord input,
            [Customer(
                ApplicationId = "{PartnerCenter.ApplicationId}",
                ApplicationSecretName = "{PartnerCenter.ApplicationSecret}",
                ApplicationTenantId = "{PartnerCenter.TenantId}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%")]List<Customer> customers,
            [CosmosDB(
                databaseName: "smartoffice",
                collectionName: "customers",
                ConnectionStringSetting = "CosmosDbConnectionString",
                CreateIfNotExists = true)]IAsyncCollector<CustomerEntry> items,
            [Queue(
                "securitysync", Connection = "StorageConnectionString")] IAsyncCollector<CustomerRecord> output,
            ILogger log)
        {
            log.LogInformation($"Processing {input.FriendlyName}");

            customers.ForEach(async (customer) =>
            {
                CustomerEntry entry = GetCustomerEntry(customer);

                await items.AddAsync(entry).ConfigureAwait(false);
                await output.AddAsync(GetCustomerRecord(entry, input.AppEndpoint)).ConfigureAwait(false);
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