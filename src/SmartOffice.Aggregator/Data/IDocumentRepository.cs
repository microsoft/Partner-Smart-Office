// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;

    /// <summary>
    /// Represents the ability to manage a document repository.
    /// </summary>
    public interface IDocumentRepository
    {
        /// <summary>
        /// Adds or updates the item.
        /// </summary>
        /// <typeparam name="TEntry">The type of entry being added.</typeparam>
        /// <param name="databaseId">The identifier of the database.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="partitionKey">The key used to partition the collection.</param>
        /// <param name="item">The item to be added.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
        Task AddOrUpdateAsync<TEntry>(string databaseId, string collectionId, string partitionKey, TEntry item);

        /// <summary>
        /// Adds or updates the collection of items.
        /// </summary>
        /// <typeparam name="TEntry">The type of entry being added.</typeparam>
        /// <param name="databaseId">The identifier of the database.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="partitionKey">The key used to partition the collection.</param>
        /// <param name="items">The collection of items to be added.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
        Task AddOrUpdateAsync<TEntry>(string databaseId, string collectionId, string partitionKey, IEnumerable<TEntry> items);

        /// <summary>
        /// Queries the collection for entries.
        /// </summary>
        /// <typeparam name="TEntry">The type of entry being queried.</typeparam>
        /// <param name="databaseId">The identifier of the database.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="partitionKey">The key used to partition the collection.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="crossPartitionQuery">A flag indicating whether or not the query should be performed across partitions.</param>
        /// <returns>A collection of entries that fulfill the query.</returns>
        Task<List<TEntry>> QueryAsync<TEntry>(string databaseName, string collectionName, string partitionKey, SqlQuerySpec query, bool crossPartitionQuery);
    }
}