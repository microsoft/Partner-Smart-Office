// -----------------------------------------------------------------------
// <copyright file="CustomersAsyncCollector.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings.Collectors
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Models;
    using Services;

    internal class CustomersAsyncCollector : IAsyncCollector<List<Customer>>
    {
        /// <summary>
        /// Name of the customers storage queue.
        /// </summary>
        private const string CustomersQueueName = "customers";

        /// <summary>
        /// The instance of the <see cref="CustomersAttribute" /> class assoicated with the output binding.
        /// </summary>
        private CustomersAttribute attribute;

        /// <summary>
        /// Provides the ability to interact with Azure Cosmos DB.
        /// </summary>
        private ICosmosDbService cosmosDbService;

        /// <summary>
        /// Provides the ability to interact with Azure storage.
        /// </summary>
        private IStorageService storageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersAsyncCollector" /> class.
        /// </summary>
        /// <param name="attribute">The attribute associated with the output binding.</param>
        public CustomersAsyncCollector(CustomersAttribute attribute)
        {
            this.attribute = attribute;
            cosmosDbService = CosmosDbService.Instance;
            storageService = StorageService.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersAsyncCollector" /> class.
        /// </summary>
        /// <param name="attribute">The attribute associated with the output binding.</param>
        /// <param name="cosmosDbService">Provides the ability to interact with Azure Cosmos DB.</param>
        /// <param name="storageService">Provides the ability to interact with Azure storage.</param>
        public CustomersAsyncCollector(CustomersAttribute attribute, ICosmosDbService cosmosDbService, IStorageService storageService)
        {
            this.attribute = attribute;
            this.cosmosDbService = cosmosDbService;
            this.storageService = storageService;
        }

        /// <summary>
        /// Adds an item to the <see cref="IAsyncCollector{T}"/>.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the aysnchronous operation.</returns>
        public async Task AddAsync(List<Customer> item, CancellationToken cancellationToken = default(CancellationToken))
        {
            await cosmosDbService.ExecuteStoredProcedureAsync(
                DataConstants.DatabaseId,
                DataConstants.CustomersCollectionId,
                DataConstants.BulkImportStoredProcedureId,
                item);

            foreach (Customer customer in item)
            {
                await storageService.WriteToQueueAsync(CustomersQueueName, customer).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Flush all the events accumulated so far. 
        /// This can be an empty operation if the messages are not batched. 
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the aysnchronous operation.</returns>
        public async Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.FromResult(0);
        }
    }
}