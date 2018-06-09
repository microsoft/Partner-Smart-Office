// -----------------------------------------------------------------------
// <copyright file="Environments.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host;
    using Bindings;
    using Converters;
    using Data;
    using Models;
    using Models.Graph;
    using Models.PartnerCenter;
    using Models.PartnerCenter.AuditRecords;
    using Models.PartnerCenter.Customers;
    using Models.PartnerCenter.Offers;
    using Models.PartnerCenter.Orders;
    using Models.PartnerCenter.Subscriptions;
    using Models.PartnerCenter.Utilizations;
    using Newtonsoft.Json;
    using Services;
    using Services.PartnerCenter;

    /// <summary>
    /// Contains the defintion for the Azure functions related to environments.
    /// </summary>
    public static class Environments
    {
        /// <summary>
        /// Azure function that process customers that are written to the customers storage queue.
        /// </summary>
        /// <param name="customerDetail">Customer event details that were written to the customers storage queue.</param>
        /// <param name="customerRepository">A data repository linked to the subscriptions collection.</param>
        /// <param name="subscriptionRepository">A data repository linked to the subscriptions collection.</param>
        /// <param name="partner">Provides the ability to interact with Partner Center.</param>
        /// <param name="storage">An instance of the <see cref="StorageService" /> class that is authenticated.</param>
        /// <param name="log">Provides the ability to log trace messages.</param>
        /// <returns>An instance of the <see cref="Task" /> that represents an asynchronous operation.</returns>
        [FunctionName("ProcessCustomer")]
        public static async Task ProcessCustomerAsync(
            [QueueTrigger(OperationConstants.CustomersQueueName, Connection = "StorageConnectionString")]ProcessCustomerDetail customerDetail,
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
                DataType = typeof(SubscriptionDetail),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<SubscriptionDetail> subscriptionRepository,
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
            IEnumerable<AuditRecord> auditRecords;
            List<SubscriptionDetail> subscriptions;
            int period;

            try
            {
                log.Info($"Attempting to process information for {customerDetail.Customer.Id}.");

                // Configure the value indicating the number of days of score results to retrieve starting from current date.
                period = (customerDetail.Customer.LastProcessed == null) ? 30 : (DateTimeOffset.UtcNow - customerDetail.Customer.LastProcessed).Value.Days;

                // Ensure that the period is at least 1 or greater. This ensure the request to retrieve data is succesfully. 
                if (period < 1)
                {
                    period = 1;
                }

                if (period >= 30)
                {
                    period = 30;

                    try
                    {
                        customerDetail.Customer.ProcessException = null;
                        subscriptions = await GetSubscriptionsAsync(
                            partner,
                            customerDetail.Customer.Id).ConfigureAwait(false);
                    }
                    catch (ServiceClientException ex)
                    {
                        customerDetail.Customer.ProcessException = ex;
                        subscriptions = null;

                        log.Warning($"Encountered an exception when processing {customerDetail.Customer.Id}. Check the customer record for more information.");
                    }
                }
                else
                {
                    // Obtain a list of audit records for the specified customer that happened since the customer was last processed.
                    auditRecords = await auditRecordRepository.GetAsync(
                        r => r.CustomerId == customerDetail.Customer.Id
                            && r.OperationDate >= customerDetail.Customer.LastProcessed,
                        customerDetail.Customer.EnvironmentId).ConfigureAwait(false);

                    // Since the period is less than 30 we can utilize the audit logs to reconstruct any subscriptions that were created.
                    subscriptions = await BuildUsingAuditRecordsAsync(
                        auditRecords,
                        subscriptionRepository,
                        partner,
                        customerDetail.Customer).ConfigureAwait(false);
                }

                if (subscriptions?.Count > 0)
                {
                    await subscriptionRepository.AddOrUpdateAsync(
                        subscriptions,
                        customerDetail.Customer.Id).ConfigureAwait(false);
                }

                if (customerDetail.Customer.ProcessException == null)
                {
                    await storage.WriteToQueueAsync(
                        OperationConstants.SecurityQueueName,
                        new SecurityDetail
                        {
                            AppEndpoint = customerDetail.AppEndpoint,
                            Customer = customerDetail.Customer,
                            Period = period.ToString(CultureInfo.InvariantCulture)
                        }).ConfigureAwait(false);

                    if (customerDetail.ProcessAzureUsage)
                    {
                        foreach (SubscriptionDetail subscription in subscriptions.Where(s => s.BillingType == BillingType.Usage))
                        {
                            await storage.WriteToQueueAsync(
                                OperationConstants.UtilizationQueueName,
                                new ProcessSubscriptionDetail
                                {
                                    PartnerCenterEndpoint = customerDetail.PartnerCenterEndpoint,
                                    Subscription = subscription
                                }).ConfigureAwait(false);
                        }
                    }

                    customerDetail.Customer.LastProcessed = DateTimeOffset.UtcNow;
                    await customerRepository.AddOrUpdateAsync(customerDetail.Customer).ConfigureAwait(false);

                    log.Info($"Successfully processed customer {customerDetail.Customer.Id}.");
                }
            }
            finally
            {
                auditRecords = null;
                subscriptions = null;
            }
        }

        /// <summary>
        /// Azure function that process partners that are written to the partner storage queue.
        /// </summary>
        /// <param name="environment">An instance of the <see cref="EnvironmentDetail" /> class that represents the environment to process.</param>
        /// <param name="auditRecordRepository">A document repository linked to the audit records collection.</param>
        /// <param name="customerRepository">A document repository linked to the customers collection.</param>
        /// <param name="environmentRepository">A document repository linked to the environments collection.</param>
        /// <param name="partner">An instance of the <see cref="PartnerServiceClient" /> class that is authenticated.</param>
        /// <param name="storage">An instance of the <see cref="StorageService" /> class that is authenticated.</param>
        /// <param name="log">Provides the ability to log trace messages.</param>
        /// <returns>An instance of the <see cref="Task" /> that represents an asynchronous operation.</returns>
        [FunctionName("ProcessPartner")]
        public static async Task ProcessPartnerAsync(
            [QueueTrigger(OperationConstants.PartnersQueueName, Connection = "StorageConnectionString")]EnvironmentDetail environment,
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
            int days;

            try
            {
                log.Info($"Starting to process the {environment.FriendlyName} CSP environment.");

                // Calculate the number of days that have gone by, since the last successful synchronization.
                days = (environment.LastProcessed == null) ? 30 : (DateTimeOffset.UtcNow - environment.LastProcessed).Days;

                if (days >= 90)
                {
                    // Only audit records for the past 90 days are available from Partner Center.
                    days = 89;
                }
                else if (days <= 0)
                {
                    // Ensure that in all circumstances at least one day of records will be returned.
                    days = 1;
                }

                auditRecords = await GetAuditRecordsAsyc(
                    partner,
                    DateTime.UtcNow.AddDays(-days),
                    DateTime.UtcNow).ConfigureAwait(false);

                if (auditRecords.Count > 0)
                {
                    log.Info($"Importing {auditRecords.Count} audit records from the past {days} days.");

                    // Add, or update, each audit record to the database.
                    await auditRecordRepository.AddOrUpdateAsync(
                        auditRecords,
                        environment.Id).ConfigureAwait(false);
                }

                if (days >= 30)
                {
                    customers = await GetCustomersAsync(partner, environment).ConfigureAwait(false);
                }
                else
                {
                    customers = await BuildUsingAuditRecordsAsync(
                        environment,
                        auditRecords,
                        customerRepository).ConfigureAwait(false);
                }

                // Add, or update, each customer to the database.
                await customerRepository.AddOrUpdateAsync(customers).ConfigureAwait(false);

                foreach (CustomerDetail customer in customers)
                {
                    // Write the customer to the customers queue to start processing the customer.
                    await storage.WriteToQueueAsync(
                        OperationConstants.CustomersQueueName,
                        new ProcessCustomerDetail
                        {
                            AppEndpoint = environment.AppEndpoint,
                            Customer = customer,
                            PartnerCenterEndpoint = environment.PartnerCenterEndpoint,
                            ProcessAzureUsage = environment.ProcessAzureUsage
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
            }
        }

        /// <summary>
        /// Azure functions that process security related information for the tenant.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="securityAlertRepository"></param>
        /// <param name="secureScoreRepository"></param>
        /// <param name="scores"></param>
        /// <param name="alerts"></param>
        /// <param name="log">Provides the ability to log trace messages.</param>
        /// <returns>An instance of the <see cref="Task" /> that represents an asynchronous operation.</returns>
        [FunctionName("ProcessSecurity")]
        public static async Task ProcessSecurityAsync(
            [QueueTrigger(OperationConstants.SecurityQueueName, Connection = "StorageConnectionString")]SecurityDetail data,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(Alert),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<Alert> securityAlertRepository,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(SecureScore),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<SecureScore> secureScoreRepository,
            [SecureScore(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                CustomerId = "{Customer.Id}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Period = "{Period}",
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
            log.Info($"Attempting to process data for {data.Customer.Id}");

            if (data.Customer.ProcessException != null)
            {
                log.Warning($"Unable to process {data.Customer.Id} please check the customer's last exception for more information.");
                return;
            }

            if (scores?.Count > 0)
            {
                log.Info($"Importing {scores.Count} Secure Score entries for {data.Customer.Id}");
                await secureScoreRepository.AddOrUpdateAsync(
                    scores,
                    data.Customer.Id).ConfigureAwait(false);
            }

            if (alerts?.Count > 0)
            {
                log.Info($"Importing {alerts.Count} security alert entries for {data.Customer.Id}");
                await securityAlertRepository.AddOrUpdateAsync(
                    alerts,
                    data.Customer.Id).ConfigureAwait(false);
            }

            log.Info($"Successfully process data for {data.Customer.Id}");
        }

        [FunctionName("ProcessUtilization")]
        public static async Task ProcessUsageAsync(
            [QueueTrigger(OperationConstants.UtilizationQueueName, Connection = "StorageConnectionString")]ProcessSubscriptionDetail subscriptionDetail,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(UtilizationDetail),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<UtilizationDetail> repository,
            [PartnerService(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                Endpoint = "{PartnerCenterEndpoint.ServiceAddress}",
                SecretName = "{PartnerCenterEndpoint.ApplicationSecretId}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Resource = "https://graph.windows.net")]IPartnerServiceClient client,
            TraceWriter log
            )
        {
            // Subscriptions with a billing type of usage are the only ones that have utilization records.
            if (subscriptionDetail.Subscription.BillingType != BillingType.Usage)
            {
                return;
            }

            log.Info($"Requesting utilization records for {subscriptionDetail.Subscription.Id}");

            List<UtilizationDetail> records;
            ResourceCollection<AzureUtilizationRecord> utilizationRecords;

            try
            {
                utilizationRecords = await client
                    .Customers[subscriptionDetail.Subscription.TenantId]
                    .Subscriptions[subscriptionDetail.Subscription.Id]
                    .Utilization
                    .Azure
                    .QueryAsync(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow).ConfigureAwait(false);

                records = new List<UtilizationDetail>(utilizationRecords.Items
                    .Select(r => ResourceConverter.Convert<AzureUtilizationRecord, UtilizationDetail>(
                        r,
                        new Dictionary<string, string>
                        {
                            { "Id", $"{r.Resource.Id}--{r.UsageStartTime}" },
                            { "SubscriptionId", subscriptionDetail.Subscription.Id },
                            { "TenantId", subscriptionDetail.Subscription.TenantId }
                        })));

                while (utilizationRecords.Links.Next != null)
                {
                    utilizationRecords = await client
                        .Customers[subscriptionDetail.Subscription.TenantId]
                        .Subscriptions[subscriptionDetail.Subscription.Id]
                        .Utilization
                        .Azure
                        .QueryAsync(utilizationRecords.Links.Next).ConfigureAwait(false);

                    records.AddRange(utilizationRecords.Items
                        .Select(r => ResourceConverter.Convert<AzureUtilizationRecord, UtilizationDetail>(
                            r,
                            new Dictionary<string, string>
                            {
                                { "Id", $"{r.Resource.Id}--{r.UsageStartTime}" },
                                { "SubscriptionId", subscriptionDetail.Subscription.Id },
                                { "TenantId", subscriptionDetail.Subscription.TenantId }
                            })));
                }

                if (records.Count > 0)
                {
                    log.Info($"Writing {records.Count} utilization records to the repository.");

                    await repository.AddOrUpdateAsync(
                        records,
                        subscriptionDetail.Subscription.Id).ConfigureAwait(false);
                }
            }
            finally
            {
                records = null;
                utilizationRecords = null;
            }
        }

        /// <summary>
        /// Azure function that pulls environments from the configured collection and writes them to the appropriate storage queue.
        /// </summary>
        /// <param name="timerInfo">Information for the timer that triggered the function.</param>
        /// <param name="repository">A document repository linked to the enviornments collection</param>
        /// <param name="storage">An instance of the <see cref="StorageService" /> client that is authenticated.</param>
        /// <param name="log">Provides the ability to log trace messages.</param>
        /// <returns>An instance of the <see cref="Task" /> that represents an asynchronous operation.</returns>
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
            int period;

            try
            {
                if (timerInfo.IsPastDue)
                {
                    log.Info("Execution of the function is starting behind schedule.");
                }

                // Obtain a complete list of all configured environments. 
                environments = await repository.GetAsync().ConfigureAwait(false);

                if (environments.Count() == 0)
                {
                    log.Warning("No environment have been configured. Ensure that an envrionment has been created using the portal.");
                    return;
                }

                foreach (EnvironmentDetail env in environments)
                {

                    if (env.EnvironmentType == EnvironmentType.CSP)
                    {
                        // Write the environment details to the partners storage queue.
                        await storage.WriteToQueueAsync(
                            OperationConstants.PartnersQueueName,
                            env).ConfigureAwait(false);
                    }
                    else
                    {
                        // Configure the value indicating the number of days of score results to retrieve starting from current date.
                        period = (env.LastProcessed == null) ? 30 : (DateTimeOffset.UtcNow - env.LastProcessed).Days;

                        // Ensure that the period is at least 1 or greater. This ensure the request to retrieve data is succesfully. 
                        if (period < 1)
                        {
                            period = 1;
                        }

                        if (period >= 30)
                        {
                            period = 30;
                        }

                        // Write the event details to the security storage queue.
                        await storage.WriteToQueueAsync(
                        OperationConstants.SecurityQueueName,
                        new SecurityDetail
                        {
                            AppEndpoint = env.AppEndpoint,
                            Customer = new CustomerDetail
                            {
                                Id = env.Id
                            },
                            Period = period.ToString(CultureInfo.InvariantCulture)
                        }).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                environments = null;
            }
        }

        private static async Task<List<CustomerDetail>> BuildUsingAuditRecordsAsync(
            EnvironmentDetail environment,
            IEnumerable<AuditRecord> auditRecords,
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

                        resources.Add(
                            ResourceConverter.Convert<Customer, CustomerDetail>(
                                resource,
                                new Dictionary<string, string> { { "EnvironmentId", environment.Id } }));
                    }
                    else if (record.OperationType == OperationType.UpdateCustomerBillingProfile)
                    {
                        control = resources.Single(c => c.Id == record.CustomerId);
                        control.BillingProfile = JsonConvert.DeserializeObject<CustomerBillingProfile>(record.ResourceNewValue);
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
            IEnumerable<AuditRecord> auditRecords,
            IDocumentRepository<SubscriptionDetail> repository,
            IPartnerServiceClient client,
            CustomerDetail customer)
        {
            IEnumerable<AuditRecord> filteredRecords;
            List<SubscriptionDetail> fromOrders;
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
                    .GetAsync(r => r.TenantId == customer.Id, customer.Id)
                    .ConfigureAwait(false);

                foreach (AuditRecord record in filteredRecords)
                {
                    if (record.ResourceType == ResourceType.Order)
                    {
                        fromOrders = await ConvertToSubscriptionDetailsAsync(
                            client,
                            customer,
                            JsonConvert.DeserializeObject<Order>(record.ResourceNewValue)).ConfigureAwait(false);

                        if (fromOrders != null)
                        {
                            resources.AddRange(fromOrders);
                        }
                    }
                    else if (record.ResourceType == ResourceType.Subscription)
                    {
                        resource = JsonConvert.DeserializeObject<Subscription>(record.ResourceNewValue);
                        control = resources.SingleOrDefault(r => r.Id.Equals(resource.Id, StringComparison.InvariantCultureIgnoreCase));

                        if (control != null)
                        {
                            resources.Remove(control);
                        }

                        resources.Add(
                            ResourceConverter.Convert<Subscription, SubscriptionDetail>(
                                resource,
                                new Dictionary<string, string> { { "TenantId", customer.Id } }));
                    }
                }

                return resources;
            }
            finally
            {
                control = null;
                filteredRecords = null;
                fromOrders = null;
                resource = null;
            }
        }

        private static IEnumerable<DateTime> ChunkDate(DateTime startDate, DateTime endDate, int size)
        {
            while (startDate < endDate)
            {
                yield return startDate;

                startDate = startDate.AddDays(size);
            }
        }

        private static async Task<List<SubscriptionDetail>> ConvertToSubscriptionDetailsAsync(
            IPartnerServiceClient client,
            CustomerDetail customer,
            Order order)
        {
            DateTime effectiveStartDate;
            DateTimeOffset creationDate;
            List<SubscriptionDetail> details;
            Offer offer;

            try
            {
                if (order.BillingCycle == BillingCycleType.OneTime)
                {
                    return null;
                }

                details = new List<SubscriptionDetail>();

                foreach (OrderLineItem lineItem in order.LineItems)
                {
                    creationDate = order.CreationDate.Value;

                    effectiveStartDate = new DateTime(
                            creationDate.UtcDateTime.Year,
                            creationDate.UtcDateTime.Month,
                            creationDate.UtcDateTime.Day);

                    offer = await client.Offers
                        .ByCountry(customer.BillingProfile.DefaultAddress.Country).ById(lineItem.OfferId)
                        .GetAsync().ConfigureAwait(false);

                    details.Add(new SubscriptionDetail
                    {
                        AutoRenewEnabled = offer.IsAutoRenewable,
                        BillingCycle = order.BillingCycle,
                        BillingType = offer.Billing,
                        CommitmentEndDate = (offer.Billing == BillingType.License) ?
                            effectiveStartDate.AddYears(1) : DateTime.Parse("9999-12-14T00:00:00Z", CultureInfo.CurrentCulture),
                        CreationDate = creationDate.UtcDateTime,
                        EffectiveStartDate = effectiveStartDate,
                        FriendlyName = lineItem.FriendlyName,
                        Id = lineItem.SubscriptionId,
                        OfferId = lineItem.OfferId,
                        OfferName = offer.Name,
                        ParentSubscriptionId = lineItem.ParentSubscriptionId,
                        PartnerId = lineItem.PartnerIdOnRecord,
                        Quantity = lineItem.Quantity,
                        Status = SubscriptionStatus.Active,
                        SuspensionReasons = null,
                        TenantId = customer.Id,
                        UnitType = offer.UnitType
                    });
                }

                return details;
            }
            finally
            {
                offer = null;
            }
        }

        private static async Task<List<AuditRecord>> GetAuditRecordsAsyc(
            IPartnerServiceClient client,
            DateTime startDate,
            DateTime endDate)
        {
            List<AuditRecord> auditRecords;
            SeekBasedResourceCollection<AuditRecord> seekAuditRecords;

            try
            {
                auditRecords = new List<AuditRecord>();

                foreach (DateTime date in ChunkDate(startDate, endDate, 30))
                {
                    // Request the audit records for the previous day from Partner Center.
                    seekAuditRecords = await client.AuditRecords.QueryAsync(date).ConfigureAwait(false);

                    auditRecords.AddRange(seekAuditRecords.Items);

                    while (seekAuditRecords.Links.Next != null)
                    {
                        // Request the next page of audit records from Partner Center.
                        seekAuditRecords = await client.AuditRecords.QueryAsync(seekAuditRecords.Links.Next).ConfigureAwait(false);

                        auditRecords.AddRange(seekAuditRecords.Items);
                    }
                }

                return auditRecords;
            }
            finally
            {
                seekAuditRecords = null;
            }
        }

        /// <summary>
        /// Gets a complete list of customers associated with the partner. 
        /// </summary>
        /// <param name="client">Provides the ability to interact with Partner Center.</param>
        /// <param name="environment">The environment that owns the customers be requesteed.</param>
        /// <returns>A list of customers associated with the partner.</returns>
        private static async Task<List<CustomerDetail>> GetCustomersAsync(IPartnerServiceClient client, EnvironmentDetail environment)
        {
            Customer customer;
            List<CustomerDetail> customers;
            SeekBasedResourceCollection<Customer> seekCustomers;

            try
            {
                // Request a list of customers from Partner Center.
                seekCustomers = await client.Customers.GetAsync().ConfigureAwait(false);

                customers = new List<CustomerDetail>(
                    seekCustomers.Items.Select(c => ResourceConverter.Convert<Customer, CustomerDetail>(c)));

                while (seekCustomers.Links.Next != null)
                {
                    // Request the next page of customers from Partner Center.
                    seekCustomers = await client.Customers.GetAsync(seekCustomers.Links.Next).ConfigureAwait(false);

                    customers.AddRange(seekCustomers.Items.Select(c => ResourceConverter.Convert<Customer, CustomerDetail>(c)));
                }

                foreach (CustomerDetail c in customers)
                {
                    try
                    {
                        customer = await client.Customers[c.Id].GetAsync().ConfigureAwait(false);
                        c.BillingProfile = customer.BillingProfile;
                        c.EnvironmentId = environment.Id;
                    }
                    catch (ServiceClientException ex)
                    {
                        c.ProcessException = ex;
                    }
                }

                return customers;
            }
            finally
            {
                seekCustomers = null;
            }
        }

        /// <summary>
        /// Gets a list of subscriptions for the specified customer.
        /// </summary>
        /// <param name="client">Provides the ability to interact with Partner Center.</param>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>A list of subscriptions for the specified customer.</returns>
        private static async Task<List<SubscriptionDetail>> GetSubscriptionsAsync(IPartnerServiceClient client, string customerId)
        {
            List<SubscriptionDetail> subscriptions;
            SeekBasedResourceCollection<Subscription> seekSubscriptions;

            try
            {

                // Request a list of subscriptions from the Partner Center API.
                seekSubscriptions = await client.Customers.ById(customerId).Subscriptions.GetAsync().ConfigureAwait(false);

                subscriptions = new List<SubscriptionDetail>(
                    seekSubscriptions.Items
                    .Select(s => ResourceConverter.Convert<Subscription, SubscriptionDetail>(
                        s,
                        new Dictionary<string, string> { { "TenantId", customerId } })));

                while (seekSubscriptions.Links.Next != null)
                {
                    // Request the next page of subscriptions from the Partner Center API.
                    seekSubscriptions = await client.Customers
                        .ById(customerId)
                        .Subscriptions
                        .GetAsync(seekSubscriptions.Links.Next).ConfigureAwait(false);

                    subscriptions.AddRange(seekSubscriptions.Items
                        .Select(s => ResourceConverter.Convert<Subscription, SubscriptionDetail>(
                            s,
                            new Dictionary<string, string> { { "TenantId", customerId } })));
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