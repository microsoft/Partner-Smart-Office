﻿// -----------------------------------------------------------------------
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
        public static async Task ProcessCustomerAsync(
            [QueueTrigger(CustomersQueue, Connection = "StorageConnectionString")]CustomerDetail customer,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(SecureScore),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<SecureScore> repository,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(Alert),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<Alert> securityAlerts,
            [SecureScore(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                CustomerId = "{Id}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Period = 1,
                Resource = "{AppEndpoint.ServiceAddress}",
                SecretName = "{AppEndpoint.ApplicationSecretId}")]List<SecureScore> scores,
            [SecurityAlerts(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                CustomerId = "{Id}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Resource = "{AppEndpoint.ServiceAddress}",
                SecretName = "{AppEndpoint.ApplicationSecretId}")]List<Alert> alerts,
            TraceWriter log)
        {
            log.Info($"Processing data for {customer.Id}");

            if (scores?.Count > 0)
            {
                log.Info($"Importing {scores.Count} Secure Score entries for {customer.Id}...");
                await repository.AddOrUpdateAsync(scores).ConfigureAwait(false);
            }

            if (alerts?.Count > 0)
            {
                log.Info($"Importing {alerts.Count} security alert entries for {customer.Id}...");
                await securityAlerts.AddOrUpdateAsync(alerts).ConfigureAwait(false);
            }

            log.Info($"Successfully process data for {customer.Id}");
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
                DataType = typeof(Customer),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<Customer> customerRepository,
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
            List<Customer> customers;
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

                if (environment?.LastProcessed < DateTimeOffset.UtcNow.AddDays(30))
                {
                    customers = await BuildUsingAuditRecordsAsync(
                        auditRecords,
                        customerRepository,
                        ResourceType.Customer).ConfigureAwait(false);
                }
                else
                {
                    customers = await GetCustomersAsync(partner).ConfigureAwait(false);
                }

                // Add, or update, each customer to the database.
                await customerRepository.AddOrUpdateAsync(customers).ConfigureAwait(false);

                foreach (Customer customer in customers)
                {
                    // Write the customer to the customers queue to start processing the customer.
                    await storage.WriteToQueueAsync(
                        CustomersQueue,
                        new CustomerDetail
                        {
                            AppEndpoint = environment.AppEndpoint,
                            Id = customer.Id
                        }).ConfigureAwait(false);
                }

                log.Info($"Successfully process the {environment.FriendlyName} CSP environment.");
            }
            finally
            {
                auditRecords = null;
                customers = null;
                seekAuditRecords = null;
            }
        }

        /// <summary>
        /// Azure function that pulls each envrionment from the database and enqueues them for processing.
        /// </summary>
        /// <param name="timerInfo"></param>
        /// <param name="repository"></param>
        /// <param name="storage"></param>
        /// <param name="log"></param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
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
                        // Extract the customer details and write them to the customers storage queue.
                        await storage.WriteToQueueAsync(
                            CustomersQueue,
                            new CustomerDetail
                            {
                                AppEndpoint = env.AppEndpoint,
                                Id = env.Id
                            }).ConfigureAwait(false);
                    }

                }
            }
            finally
            {
                environments = null;
            }
        }

        private static async Task<List<Customer>> GetCustomersAsync(IPartnerServiceClient partner)
        {
            List<Customer> customers;
            SeekBasedResourceCollection<Customer> seekCustomers;

            try
            {
                // Request a list of customers from the Partner Center API.
                seekCustomers = await partner.Customers.GetAsync().ConfigureAwait(false);

                customers = new List<Customer>(seekCustomers.Items);

                while (seekCustomers.Links.Next != null)
                {
                    // Request the next page of customers from the Partner Center API.
                    seekCustomers = await partner.Customers.GetAsync(seekCustomers.Links.Next).ConfigureAwait(false);

                    customers.AddRange(seekCustomers.Items);
                }

                return customers;
            }
            finally
            {
                seekCustomers = null;
            }
        }

        private static async Task<List<TResource>> BuildUsingAuditRecordsAsync<TResource>(
            List<AuditRecord> auditRecords,
            IDocumentRepository<TResource> repository,
            ResourceType resourceType) where TResource : StandardResource
        {
            IEnumerable<AuditRecord> filteredRecords;
            List<TResource> resources;
            TResource control;
            TResource resource;

            try
            {
                // Extract a list of audit records that are scope to the defined resource type and were successful.
                filteredRecords = auditRecords
                    .Where(r => r.ResourceType == resourceType && r.OperationStatus == OperationStatus.Succeeded)
                    .OrderBy(r => r.OperationDate);

                // Obtain a list of existing resources from the data repository.
                resources = await repository.GetAsync().ConfigureAwait(false);

                foreach (AuditRecord record in filteredRecords)
                {
                    if (record.ResourceType == ResourceType.Customer)
                    {
                        if (record.OperationType == OperationType.AddCustomer)
                        {
                            resource = JsonConvert.DeserializeObject<TResource>(record.ResourceNewValue);
                            control = resources.SingleOrDefault(r => r.Id.Equals(resource.Id, StringComparison.InvariantCultureIgnoreCase));

                            if (control != null)
                            {
                                resources.Remove(control);
                            }

                            resources.Add(resource);
                        }
                    }
                    else if (record.ResourceType == ResourceType.Order)
                    {
                        // TODO - Convert a new order to a subscription.
                    }
                    else if (record.ResourceType == ResourceType.Subscription)
                    {
                        resource = JsonConvert.DeserializeObject<TResource>(record.ResourceNewValue);
                        control = resources.SingleOrDefault(r => r.Id.Equals(resource.Id, StringComparison.InvariantCultureIgnoreCase));

                        if (control != null)
                        {
                            resources.Remove(control);
                        }

                        resources.Add(resource);
                    }
                }

                return resources;
            }
            finally
            {
                filteredRecords = null;
            }
        }
    }
}