// -----------------------------------------------------------------------
// <copyright file="Environments.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host;
    using Bindings;
    using Data;
    using Models;
    using Models.Graph;
    using Models.PartnerCenter;
    using Newtonsoft.Json;
    using Services;
    using Services.PartnerCenter;

    /// <summary>
    /// Contains the defintion for the Azure functions related to environments.
    /// </summary>
    public static class Environments
    {
        /// <summary>
        /// Name of the customers storage queue.
        /// </summary>
        private const string CustomersQueue = "customers";

        /// <summary>
        /// Name of the partners storage queue.
        /// </summary>
        private const string PartnersQueue = "partners";

        [FunctionName("ProcessCustomer")]
        public static async Task ProcessCspCustomerAsync(
            [QueueTrigger(CustomersQueue, Connection = "StorageConnectionString")]XEvent data,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(Alert),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<Alert> securityAlertRepository,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(CustomerDetail),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<CustomerDetail> customerRepository,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(SecureScore),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<SecureScore> secureScoreRepository,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(SubscriptionDetail),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<SubscriptionDetail> subscriptionRepository,
            [PartnerService(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                Endpoint = "{PartnerCenterEndpoint.ServiceAddress}",
                SecretName = "{PartnerCenterEndpoint.ApplicationSecretId}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Resource = "https://graph.windows.net")]IPartnerServiceClient partner,
            [SecureScore(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                CustomerId = "{Customer.Id}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Period = 1,
                Resource = "{AppEndpoint.ServiceAddress}",
                SecretName = "{AppEndpoint.ApplicationSecretId}")]List<SecureScore> scores,
            [SecurityAlerts(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                CustomerId = "{Customer.Id}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Resource = "{AppEndpoint.ServiceAddress}",
                SecretName = "{AppEndpoint.ApplicationSecretId}")]List<Alert> alerts,
            TraceWriter log)
        {
            List<SubscriptionDetail> subscriptions;

            try
            {
                log.Info($"Processing data for {data.Customer.Id}");

                if (data.Customer.LastProcessed == null || (DateTimeOffset.UtcNow - data.Customer.LastProcessed).Value.TotalDays >= 30)
                {
                    subscriptions = await GetSubscriptionsAsync(partner, data.Customer.Id).ConfigureAwait(false);
                }
                else
                {
                    subscriptions = await BuildUsingAuditRecordsAsync(
                        data.AuditRecords,
                        subscriptionRepository,
                        data.Customer.Id).ConfigureAwait(false);
                }

                if (subscriptions.Count > 0)
                {
                    await subscriptionRepository.AddOrUpdateAsync(subscriptions).ConfigureAwait(false);

                    if (scores?.Count > 0)
                    {
                        log.Info($"Importing {scores.Count} Secure Score entries for {data.Customer.Id}");
                        await secureScoreRepository.AddOrUpdateAsync(scores).ConfigureAwait(false);
                    }

                    if (alerts?.Count > 0)
                    {
                        log.Info($"Importing {alerts.Count} security alert entries for {data.Customer.Id}");
                        await securityAlertRepository.AddOrUpdateAsync(alerts).ConfigureAwait(false);
                    }
                }

                data.Customer.LastProcessed = DateTimeOffset.UtcNow;

                await customerRepository.AddOrUpdateAsync(data.Customer).ConfigureAwait(false);

                log.Info($"Successfully process data for {data.Customer.Id}");
            }
            finally
            {
                subscriptions = null;
            }
        }

        [FunctionName("ProcessPartner")]
        public static async Task ProcessPartnerAsync(
            [QueueTrigger(PartnersQueue, Connection = "StorageConnectionString")]EnvironmentDetail environment,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(AuditRecord),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<AuditRecord> auditRecordRepository,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(CustomerDetail),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<CustomerDetail> customerRepository,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(EnvironmentDetail),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<EnvironmentDetail> environmentRepository,
            [PartnerService(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                Endpoint = "{PartnerCenterEndpoint.ServiceAddress}",
                SecretName = "{PartnerCenterEndpoint.ApplicationSecretId}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Resource = "https://graph.windows.net")]IPartnerServiceClient partner,
            [StorageService(
                ConnectionStringName = "StorageConnectionString",
                KeyVaultEndpoint = "KeyVaultEndpoint")]IStorageService storage,
            TraceWriter log)
        {
            List<AuditRecord> auditRecords;
            List<CustomerDetail> customers;
            SeekBasedResourceCollection<AuditRecord> seekAuditRecords;

            try
            {
                log.Info($"Starting to process the {environment.FriendlyName} CSP environment.");

                // Request the audit records for the previous day from the Partner Center API.
                seekAuditRecords = await partner.AuditRecords.QueryAsync(DateTime.Now.AddDays(-1)).ConfigureAwait(false);

                auditRecords = new List<AuditRecord>(seekAuditRecords.Items);

                while (seekAuditRecords.Links.Next != null)
                {
                    // Request the next page of audit records from the Partner Center API.
                    seekAuditRecords = await partner.AuditRecords.QueryAsync(seekAuditRecords.Links.Next).ConfigureAwait(false);

                    auditRecords.AddRange(seekAuditRecords.Items);
                }

                if (auditRecords.Count > 0)
                {
                    // Add, or update, each audit record to the database.
                    await auditRecordRepository.AddOrUpdateAsync(auditRecords).ConfigureAwait(false);
                }

                if ((DateTimeOffset.UtcNow - environment?.LastProcessed).Value.TotalDays >= 30)
                {
                    customers = await GetCustomersAsync(partner).ConfigureAwait(false);
                }
                else
                {
                    customers = await BuildUsingAuditRecordsAsync(
                        auditRecords,
                        customerRepository).ConfigureAwait(false);
                }

                // Add, or update, each customer to the database.
                await customerRepository.AddOrUpdateAsync(customers).ConfigureAwait(false);

                foreach (CustomerDetail customer in customers)
                {
                    // Write the customer to the customers queue to start processing the customer.
                    await storage.WriteToQueueAsync(
                        CustomersQueue,
                        new XEvent
                        {
                            AppEndpoint = environment.AppEndpoint,
                            AuditRecords = auditRecords
                                .Where(r => r.CustomerId.Equals(customer.Id, StringComparison.InvariantCultureIgnoreCase))
                                .ToList(),
                            Customer = customer,
                            PartnerCenterEndpoint = environment.PartnerCenterEndpoint
                        }).ConfigureAwait(false);
                }

                environment.LastProcessed = DateTimeOffset.UtcNow;
                await environmentRepository.AddOrUpdateAsync(environment).ConfigureAwait(false);

                log.Info($"Successfully process the {environment.FriendlyName} CSP environment.");
            }
            finally
            {
                auditRecords = null;
                customers = null;
                seekAuditRecords = null;
            }
        }

        [FunctionName("PullEnvironments")]
        public static async Task PullEnvironmentsAsync(
            [TimerTrigger("0 0 9 * * *")]TimerInfo timerInfo,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(EnvironmentDetail),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<EnvironmentDetail> repository,
            [StorageService(
                ConnectionStringName = "StorageConnectionString",
                KeyVaultEndpoint = "KeyVaultEndpoint")]IStorageService storage,
            TraceWriter log)
        {
            IEnumerable<EnvironmentDetail> environments;

            try
            {
                if (timerInfo.IsPastDue)
                {
                    log.Info("Execution of the function is starting behind schedule.");
                }

                // Obtain a complete list of all configured environments. 
                environments = await repository.GetAsync().ConfigureAwait(false);

                foreach (EnvironmentDetail env in environments)
                {

                    if (env.EnvironmentType == EnvironmentType.CSP)
                    {
                        // Write the environment details to the partners storage queue.
                        await storage.WriteToQueueAsync(PartnersQueue, env).ConfigureAwait(false);
                    }
                    else
                    {
                        // TODO - Add the logic to process direct and EA subscriptions. 
                    }

                }
            }
            finally
            {
                environments = null;
            }
        }

        private static async Task<List<CustomerDetail>> BuildUsingAuditRecordsAsync(
            List<AuditRecord> auditRecords,
            IDocumentRepository<CustomerDetail> repository)
        {
            IEnumerable<AuditRecord> filteredRecords;
            List<CustomerDetail> resources;
            CustomerDetail control;
            Customer resource;

            try
            {
                // Extract a list of audit records that are scope to the defined resource type and were successful.
                filteredRecords = auditRecords
                    .Where(r => r.ResourceType == ResourceType.Customer && r.OperationStatus == OperationStatus.Succeeded)
                    .OrderBy(r => r.OperationDate);

                resources = await repository.GetAsync().ConfigureAwait(false);

                foreach (AuditRecord record in filteredRecords)
                {
                    if (record.OperationType == OperationType.AddCustomer)
                    {
                        resource = JsonConvert.DeserializeObject<Customer>(record.ResourceNewValue);
                        control = resources.SingleOrDefault(r => r.Id.Equals(resource.Id, StringComparison.InvariantCultureIgnoreCase));

                        if (control != null)
                        {
                            resources.Remove(control);
                        }

                        resources.Add(ConvertToCustomerDetail(resource));
                    }
                }

                return resources;
            }
            finally
            {
                control = null;
                filteredRecords = null;
                resource = null;
            }
        }

        private static async Task<List<SubscriptionDetail>> BuildUsingAuditRecordsAsync(
            List<AuditRecord> auditRecords,
            IDocumentRepository<SubscriptionDetail> repository,
            string customerId)
        {
            IEnumerable<AuditRecord> filteredRecords;
            List<SubscriptionDetail> resources;
            SubscriptionDetail control;
            Subscription resource;

            try
            {
                // Extract a list of audit records that are scope to the defined resource type and were successful.
                filteredRecords = auditRecords
                    .Where(r => (r.ResourceType == ResourceType.Subscription || r.ResourceType == ResourceType.Order)
                        && r.OperationStatus == OperationStatus.Succeeded)
                    .OrderBy(r => r.OperationDate);

                resources = await repository
                    .GetAsync(r => r.TenantId.Equals(customerId, StringComparison.InvariantCultureIgnoreCase))
                    .ConfigureAwait(false);

                foreach (AuditRecord record in filteredRecords)
                {
                    if (record.ResourceType == ResourceType.Order)
                    {
                        // TODO - Convert a new order to a subscription.
                    }
                    else if (record.ResourceType == ResourceType.Subscription)
                    {
                        resource = JsonConvert.DeserializeObject<Subscription>(record.ResourceNewValue);
                        control = resources.SingleOrDefault(r => r.Id.Equals(resource.Id, StringComparison.InvariantCultureIgnoreCase));

                        if (control != null)
                        {
                            resources.Remove(control);
                        }

                        resources.Add(ConvertToSubscriptionDetail(resource, customerId));
                    }
                }

                return resources;
            }
            finally
            {
                control = null;
                filteredRecords = null;
                resource = null;
            }

        }

        private static CustomerDetail ConvertToCustomerDetail(Customer customer)
        {
            return new CustomerDetail
            {
                CompanyProfile = customer.CompanyProfile,
                Id = customer.Id,
                LastProcessed = null
            };
        }

        private static SubscriptionDetail ConvertToSubscriptionDetail(Subscription subscription, string customerId)
        {
            return new SubscriptionDetail
            {
                AutoRenewEnabled = subscription.AutoRenewEnabled,
                BillingCycle = subscription.BillingCycle,
                BillingType = subscription.BillingType,
                CommitmentEndDate = subscription.CommitmentEndDate,
                CreationDate = subscription.CreationDate,
                EffectiveStartDate = subscription.EffectiveStartDate,
                FriendlyName = subscription.FriendlyName,
                Id = subscription.Id,
                OfferId = subscription.OfferId,
                OfferName = subscription.OfferName,
                ParentSubscriptionId = subscription.ParentSubscriptionId,
                PartnerId = subscription.PartnerId,
                Quantity = subscription.Quantity,
                Status = subscription.Status,
                SuspensionReasons = subscription.SuspensionReasons,
                TenantId = customerId,
                UnitType = subscription.UnitType
            };
        }

        private static async Task<List<CustomerDetail>> GetCustomersAsync(IPartnerServiceClient partner)
        {
            List<CustomerDetail> customers;
            SeekBasedResourceCollection<Customer> seekCustomers;

            try
            {
                // Request a list of customers from the Partner Center API.
                seekCustomers = await partner.Customers.GetAsync().ConfigureAwait(false);

                customers = new List<CustomerDetail>(seekCustomers.Items.Select(c => ConvertToCustomerDetail(c)));

                while (seekCustomers.Links.Next != null)
                {
                    // Request the next page of customers from the Partner Center API.
                    seekCustomers = await partner.Customers.GetAsync(seekCustomers.Links.Next).ConfigureAwait(false);

                    customers.AddRange(seekCustomers.Items.Select(c => ConvertToCustomerDetail(c)));
                }

                return customers;
            }
            finally
            {
                seekCustomers = null;
            }
        }

        private static async Task<List<SubscriptionDetail>> GetSubscriptionsAsync(IPartnerServiceClient partner, string customerId)
        {
            List<SubscriptionDetail> subscriptions;
            SeekBasedResourceCollection<Subscription> seekSubscriptions;

            try
            {
                // Request a list of subscriptions from the Partner Center API.
                seekSubscriptions = await partner.Customers.ById(customerId).Subscriptions.GetAsync().ConfigureAwait(false);

                subscriptions = new List<SubscriptionDetail>(
                    seekSubscriptions.Items.Select(s => ConvertToSubscriptionDetail(s, customerId)));

                while (seekSubscriptions.Links.Next != null)
                {
                    // Request the next page of subscriptions from the Partner Center API.
                    seekSubscriptions = await partner.Customers
                        .ById(customerId)
                        .Subscriptions
                        .GetAsync(seekSubscriptions.Links.Next).ConfigureAwait(false);

                    subscriptions.AddRange(seekSubscriptions.Items.Select(s => ConvertToSubscriptionDetail(s, customerId)));
                }

                return subscriptions;
            }
            finally
            {
                seekSubscriptions = null;
            }
        }
    }
}