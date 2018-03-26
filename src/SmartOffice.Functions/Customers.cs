// -----------------------------------------------------------------------
// <copyright file="Customers.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host;
    using Bindings;
    using Data;
    using Models;
    using Services;

    public static class Customers
    {
        [FunctionName("ProcessCustomers")]
        public static async Task ProcessAsync(
            [TimerTrigger("0 0 12 * * *")]TimerInfo timerInfo,
            [DataRepository(typeof(Customer))]IDocumentRepository<Customer> repository,
            [StorageService(
                ConnectionStringName = "StorageConnectionString",
                KeyVaultEndpoint = "KeyVaultEndpoint")]IStorageService storage,
            TraceWriter log)
        {
            IEnumerable<Customer> customers;

            try
            {
                customers = await repository.GetAsync().ConfigureAwait(false);

                foreach (Customer customer in customers)
                {
                    log.Info($"Processing {customer.CompanyProfile.CompanyName}...");
                    await storage.WriteToQueueAsync("customers", customer).ConfigureAwait(false);
                }
            }
            finally
            {
                customers = null;
            }
        }

        [FunctionName("PullCustomers")]
        public static async Task PullAsync(
            [TimerTrigger("0 0 10 * * *")]TimerInfo timerInfo,
            [DataRepository(typeof(Customer))]IDocumentRepository<Customer> repository,
            [PartnerService(
                ApplicationId ="PartnerCenter.ApplicationId",
                SecretName = "PartnerCenterApplicationSecret",
                ApplicationTenantId = "PartnerCenter.AccountId",
                Resource = "https://graph.windows.net")]IPartnerService partner,
            TraceWriter log)
        {
            List<Customer> customers;

            try
            {

                customers = await partner.GetCustomersAsync().ConfigureAwait(false);

                await repository.AddOrUpdateAsync(customers).ConfigureAwait(false);
            }
            finally
            {
                customers = null;
            }
        }
    }
}