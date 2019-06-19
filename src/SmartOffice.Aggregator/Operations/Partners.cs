// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.AzureMonitor;
    using Microsoft.Azure.WebJobs.Extensions.PartnerCenter;
    using Microsoft.Extensions.Logging;
    using Microsoft.Store.PartnerCenter.Models.Auditing;
    using Microsoft.Store.PartnerCenter.Models.Customers;
    using Microsoft.Store.PartnerCenter.Models.Invoices;
    using Microsoft.Store.PartnerCenter.Models.Subscriptions;
    using Models;
    using Models.Converters;
    using Telemetry;

    /// <summary>
    /// Defines the Azure Functions used process partner information.
    /// </summary>
    public class Partners
    {
        /// <summary>
        /// Provides the ability to manage a document repository.
        /// </summary>
        private readonly IDocumentRepository repository;

        /// <summary>
        /// Provides the ability to capture telemetry.
        /// </summary>
        private readonly ITelemetryProvider telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="Partners" /> class.
        /// </summary>
        /// <param name="repository">Provides the ability to manage a document repository.</param>
        /// <param name="telemetry">Provides the ability to capture telemetry.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="repository"/> is null
        /// or
        /// <paramref name="telemetry"/> is null
        /// </exception>
        public Partners(IDocumentRepository repository, ITelemetryProvider telemetry)
        {
            repository.AssertNotNull(nameof(repository));
            telemetry.AssertNotNull(nameof(telemetry));

            this.repository = repository;
            this.telemetry = telemetry;
        }

        [FunctionName(OperationConstants.PartnerFullSync)]
        public async Task PartnerFullSyncAsync(
            [QueueTrigger(OperationConstants.PartnerFullSync, Connection = OperationConstants.StorageConnectionString)]EnvironmentRecord input,
            [Customer(
                ApplicationId = "{Entry.PartnerCenterEndpoint.ApplicationId}",
                ApplicationSecretName = "{Entry.PartnerCenterEndpoint.ApplicationSecretName}",
                ApplicationTenantId = "{Entry.PartnerCenterEndpoint.TenantId}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%")]List<Customer> customers,
            [AuditRecord(
                ApplicationId = "{Entry.PartnerCenterEndpoint.ApplicationId}",
                ApplicationSecretName = "{Entry.PartnerCenterEndpoint.ApplicationSecretName}",
                ApplicationTenantId = "{Entry.PartnerCenterEndpoint.TenantId}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                EndDate = "{AuditEndDate}",
                StartDate = "{AuditStartDate}")]List<AuditRecord> records,
            [DataCollector(
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                WorkspaceId = "{Entry.WorkspaceId}",
                WorkspaceKeyName = "{Entry.WorkspaceKeyName}")]IAsyncCollector<LogData> dataCollector,
            [Queue(OperationConstants.SubscriptionSync, Connection = OperationConstants.StorageConnectionString)]IAsyncCollector<CustomerRecord> subscriptionSync,
            ILogger log)
        {
            DataEntry<CustomerEntry> entry;
            DateTime executionTime = DateTime.UtcNow;

            foreach (Customer customer in customers)
            {
                entry = GetCustomerEntry(customer, input);

                log.LogInformation($"Processing {entry.Entry.CustomerName} from the {entry.EnvironmentName} environment");

                await subscriptionSync.AddAsync(GetCustomerRecord(entry, input)).ConfigureAwait(false);
            }

            await repository.AddOrUpdateAsync(
                OperationConstants.DatabaseId,
                OperationConstants.EnvironmentsCollectionId,
                input.Id,
                customers.Select(c => GetCustomerEntry(c, input))).ConfigureAwait(false);

            await dataCollector.AddAsync(new LogData
            {
                Data = records,
                LogType = "PartnerCenterAudit",
                TimeStampField = "OperationDate"
            }).ConfigureAwait(false);

            Dictionary<string, double> eventMetrics = new Dictionary<string, double>
            {
                { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds },
                { "NumberOfAuditRecords", records.Count },
                { "NumberOfCustomers", customers.Count }
            };

            Dictionary<string, string> eventProperties = new Dictionary<string, string>
            {
                { "EnvironmentId", input.EnvironmentId }
            };

            telemetry.TrackEvent(nameof(PartnerFullSyncAsync), eventProperties, eventMetrics);
        }

        [FunctionName(OperationConstants.SubscriptionSync)]
        public async Task SubscriptionSyncAsync(
            [QueueTrigger(OperationConstants.SubscriptionSync, Connection = OperationConstants.StorageConnectionString)]CustomerRecord input,
            [Subscription(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                ApplicationSecretName = "{PartnerCenterEndpoint.ApplicationSecretName}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                CustomerId = "{Id}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%")]List<Subscription> subscriptions,
            [Queue(OperationConstants.ControlProfileSync, Connection = OperationConstants.StorageConnectionString)]IAsyncCollector<CustomerRecord> profileSync,
            [Queue(OperationConstants.SecurityEventSync, Connection = OperationConstants.StorageConnectionString)]IAsyncCollector<CustomerRecord> securitySync,
            ILogger log)
        {
            DateTime executionTime = DateTime.UtcNow;

            log.LogInformation($"Processing {input.Entry.CustomerId} from the {input.EnvironmentId} environment");

            input.Subscriptions = string.Join(",", subscriptions.Where(s => s.BillingType == BillingType.Usage).Select(s => s.Id));

            await profileSync.AddAsync(input).ConfigureAwait(false);
            await securitySync.AddAsync(input).ConfigureAwait(false);

            Dictionary<string, double> eventMetrics = new Dictionary<string, double>
            {
                { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds },
                { "NumberOfSubscriptions", subscriptions.Count }
            };

            Dictionary<string, string> eventProperties = new Dictionary<string, string>
            {
                { "CustomerId", input.Entry.CustomerId },
                { "EnvironmentId", input.EnvironmentId }
            };

            telemetry.TrackEvent(nameof(SubscriptionSyncAsync), eventProperties, eventMetrics);
        }


        private static DataEntry<CustomerEntry> GetCustomerEntry(Customer customer, EnvironmentRecord environment)
        {
            customer.AssertNotNull(nameof(customer));
            environment.AssertNotNull(nameof(environment));

            DataEntry<CustomerEntry> entry = new DataEntry<CustomerEntry>
            {
                EnvironmentId = environment.EnvironmentId,
                EnvironmentName = environment.EnvironmentName,
                Id = customer.Id
            };

            entry.Entry.CustomerId = customer.Id;
            entry.Entry.CustomerName = customer.CompanyProfile.CompanyName;

            return entry;
        }

        private static CustomerRecord GetCustomerRecord(DataEntry<CustomerEntry> entry, EnvironmentRecord environment)
        {
            CustomerRecord record;
            int period;

            entry.AssertNotNull(nameof(entry));
            environment.AssertNotNull(nameof(environment));

            record = ResourceConverter.Convert<DataEntry<CustomerEntry>, CustomerRecord>(entry);

            // Calculate the period, the number of days of Secure Score results to retrieve starting from current date, based on the last porcessed time.
            //period = (entry.LastProcessed == null) ? 30 : (DateTimeOffset.UtcNow - entry.LastProcessed).Value.Days;

            // Ensure the period is between 1 and 30. You cannot request more than the past 30 days of Secure results.
            //if (period < 1)
            //{
            //    period = 1;
            //}
            //else if (period > 30)
            //{
            //    period = 30;
            //}

            period = 30;

            record.AppEndpoint = environment.Entry.AppEndpoint;
            record.EnvironmentId = environment.Id;
            record.EnvironmentName = environment.EnvironmentName;
            record.PartnerCenterEndpoint = environment.Entry.PartnerCenterEndpoint;
            record.SecureScorePeriod = period;
            record.WorkspaceId = environment.Entry.WorkspaceId;
            record.WorkspaceKeyName = environment.Entry.WorkspaceKeyName;

            return record;
        }
    }
}