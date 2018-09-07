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
    using Data;
    using Extensions.Bindings;
    using Models;
    using Models.Graph;
    using Models.PartnerCenter;
    using Models.PartnerCenter.AuditRecords;
    using Models.PartnerCenter.Customers;
    using Models.PartnerCenter.Subscriptions;
    using Models.PartnerCenter.Utilizations;
    using ResourceConverters;
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
            [DataRepository(DataType = typeof(AuditRecord))]DocumentRepository<AuditRecord> auditRecordRepository,
            [DataRepository(DataType = typeof(CustomerDetail))]DocumentRepository<CustomerDetail> customerRepository,
            [DataRepository(DataType = typeof(SubscriptionDetail))]DocumentRepository<SubscriptionDetail> subscriptionRepository,
            [PartnerService(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                Endpoint = "{PartnerCenterEndpoint.ServiceAddress}",
                SecretName = "{PartnerCenterEndpoint.ApplicationSecretId}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                Resource = "https://graph.windows.net")]IPartnerServiceClient partner,
            [Queue(OperationConstants.SecurityQueueName, Connection = "StorageConnectionString")] ICollector<SecurityDetail> securityQueue,
            [Queue(OperationConstants.UtilizationQueueName, Connection = "StorageConnectionString")] ICollector<ProcessSubscriptionDetail> utilizationQueue,
            TraceWriter log)
        {
            IEnumerable<AuditRecord> auditRecords;
            List<SubscriptionDetail> subscriptions;
            int period;

            try
            {
                log.Info($"Attempting to process information for {customerDetail.Customer.Id}.");

                if (customerDetail.Customer.RemovedFromPartnerCenter)
                {
                    // The customer no longer has relationship with the partner. So, it should not be processed.
                    return;
                }

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

                    subscriptions = await subscriptionRepository
                        .GetAsync(s => s.TenantId == customerDetail.Customer.Id, customerDetail.Customer.Id)
                        .ConfigureAwait(false);

                    // Since the period is less than 30 we can utilize the audit logs to reconstruct any subscriptions that were created.
                    subscriptions = await AuditRecordConverter.ConvertAsync(
                        partner,
                        auditRecords,
                        subscriptions,
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
                    securityQueue.Add(new SecurityDetail
                    {
                        AppEndpoint = customerDetail.AppEndpoint,
                        Customer = customerDetail.Customer,
                        Period = period.ToString(CultureInfo.InvariantCulture)
                    });

                    if (customerDetail.ProcessAzureUsage)
                    {
                        foreach (SubscriptionDetail subscription in subscriptions.Where(s => s.BillingType == BillingType.Usage))
                        {
                            utilizationQueue.Add(new ProcessSubscriptionDetail
                            {
                                PartnerCenterEndpoint = customerDetail.PartnerCenterEndpoint,
                                Subscription = subscription
                            });
                        }
                    }

                    customerDetail.Customer.LastProcessed = DateTimeOffset.UtcNow;
                }

                await customerRepository.AddOrUpdateAsync(customerDetail.Customer).ConfigureAwait(false);

                log.Info($"Successfully processed customer {customerDetail.Customer.Id}. Exception(s): {(customerDetail.Customer.ProcessException != null ? "yes" : "no")}");
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
        /// <param name="client">An instance of the <see cref="PartnerServiceClient" /> class that is authenticated.</param>
        /// <param name="storage">An instance of the <see cref="StorageService" /> class that is authenticated.</param>
        /// <param name="log">Provides the ability to log trace messages.</param>
        /// <returns>An instance of the <see cref="Task" /> that represents an asynchronous operation.</returns>
        [FunctionName("ProcessPartner")]
        public static async Task ProcessPartnerAsync(
            [QueueTrigger(OperationConstants.PartnersQueueName, Connection = "StorageConnectionString")]EnvironmentDetail environment,
            [DataRepository(DataType = typeof(AuditRecord))]DocumentRepository<AuditRecord> auditRecordRepository,
            [DataRepository(DataType = typeof(CustomerDetail))]DocumentRepository<CustomerDetail> customerRepository,
            [DataRepository(
                DataType = typeof(EnvironmentDetail))]DocumentRepository<EnvironmentDetail> environmentRepository,
            [PartnerService(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                Endpoint = "{PartnerCenterEndpoint.ServiceAddress}",
                SecretName = "{PartnerCenterEndpoint.ApplicationSecretId}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
                Resource = "https://graph.windows.net")]IPartnerServiceClient client,
            [Queue(OperationConstants.CustomersQueueName, Connection = "StorageConnectionString")] ICollector<ProcessCustomerDetail> customerQueue,
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

                if (days >= 30)
                {
                    customers = await GetCustomersAsync(client, environment).ConfigureAwait(false);
                }
                else
                {
                    auditRecords = await GetAuditRecordsAsyc(
                        client,
                        DateTime.UtcNow.AddDays(-days),
                        DateTime.UtcNow).ConfigureAwait(false);

                    if (auditRecords.Count > 0)
                    {
                        log.Info($"Importing {auditRecords.Count} audit records available between now and the previous day.");

                        // Add, or update, each audit record to the database.
                        await auditRecordRepository.AddOrUpdateAsync(
                            auditRecords,
                            environment.Id).ConfigureAwait(false);
                    }

                    customers = await customerRepository.GetAsync().ConfigureAwait(false);

                    customers = await AuditRecordConverter.ConvertAsync(
                        client,
                        auditRecords,
                        customers,
                        new Dictionary<string, string> { { "EnvironmentId", environment.Id } }).ConfigureAwait(false);
                }

                // Add, or update, each customer to the database.
                await customerRepository.AddOrUpdateAsync(customers).ConfigureAwait(false);

                foreach (CustomerDetail customer in customers)
                {
                    // Write the customer to the customers queue to start processing the customer.
                    customerQueue.Add(new ProcessCustomerDetail
                    {
                        AppEndpoint = environment.AppEndpoint,
                        Customer = customer,
                        PartnerCenterEndpoint = environment.PartnerCenterEndpoint,
                        ProcessAzureUsage = environment.ProcessAzureUsage
                    });
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
                DataType = typeof(Alert))]DocumentRepository<Alert> securityAlertRepository,
            [DataRepository(DataType = typeof(SecureScore))]DocumentRepository<SecureScore> secureScoreRepository,
            [SecureScore(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                CustomerId = "{Customer.Id}",
                Period = "{Period}",
                Resource = "{AppEndpoint.ServiceAddress}",
                SecretName = "{AppEndpoint.ApplicationSecretId}")]List<SecureScore> scores,
            [SecurityAlerts(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                CustomerId = "{Customer.Id}",
                Resource = "{AppEndpoint.ServiceAddress}",
                SecretName = "{AppEndpoint.ApplicationSecretId}")]List<Alert> alerts,
            TraceWriter log)
        {
            if (data.Customer.ProcessException != null)
            {
                log.Warning($"Unable to process {data.Customer.Id} please check the customer's last exception for more information.");
                return;
            }

            if (scores?.Count > 0)
            {
                log.Info($"Importing {scores.Count} Secure Score entries from the past {data.Period} periods for {data.Customer.Id}");

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
                DataType = typeof(UtilizationDetail))]DocumentRepository<UtilizationDetail> repository,
            [PartnerService(
                ApplicationId = "{PartnerCenterEndpoint.ApplicationId}",
                Endpoint = "{PartnerCenterEndpoint.ServiceAddress}",
                SecretName = "{PartnerCenterEndpoint.ApplicationSecretId}",
                ApplicationTenantId = "{PartnerCenterEndpoint.TenantId}",
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

                records = new List<UtilizationDetail>();

                if (utilizationRecords.TotalCount > 0)
                {
                    records.AddRange(utilizationRecords.Items
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
                DataType = typeof(EnvironmentDetail))]DocumentRepository<EnvironmentDetail> repository,
            [Queue(OperationConstants.PartnersQueueName, Connection = "StorageConnectionString")] ICollector<EnvironmentDetail> partnerQueue,
            [Queue(OperationConstants.SecurityQueueName, Connection = "StorageConnectionString")] ICollector<SecurityDetail> securityQueue,
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
                        partnerQueue.Add(env);
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
                        securityQueue.Add(new SecurityDetail
                        {
                            AppEndpoint = env.AppEndpoint,
                            Customer = new CustomerDetail
                            {
                                Id = env.Id
                            },

                            Period = period.ToString(CultureInfo.InvariantCulture)
                        });
                    }
                }
            }
            finally
            {
                environments = null;
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
        private static async Task<List<CustomerDetail>> GetCustomersAsync(
            IPartnerServiceClient client,
            EnvironmentDetail environment)
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

                        if (c.RemovedFromPartnerCenter == true)
                        {
                            c.LastProcessed = null;
                        }

                        c.RemovedFromPartnerCenter = false;
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