// -----------------------------------------------------------------------
// <copyright file="ProcessCustomers.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Bindings;
    using Data;
    using Azure.WebJobs.Host;
    using Models;
    using Services;

    public static class ProcessCustomers
    {
        [FunctionName("ProcessCustomers")]
        public static async Task RunAsync(
            [TimerTrigger("0 0 12 * * *")]TimerInfo timerInfo,
            [CustomersRepository]IDocumentRepository<Customer> repository,
            [StorageService(
                ConnectionStringName = "StorageConnectionString",
                KeyVaultEndpoint = "KeyVaultEndpoint"
            )]IStorageService storage,
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
    }
}