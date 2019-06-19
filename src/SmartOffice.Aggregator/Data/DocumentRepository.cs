// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using Models.Converters;
    using Models.Resolvers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Provides the ability to manage a document repository.
    /// </summary>
    public class DocumentRepository : IDocumentRepository
    {
        /// <summary>
        /// HTTP status code returned when the collection has exceeded the provisioned
        /// throughput limit. The request should be retried after the server specified 
        /// retry after duration.
        /// </summary>
        private const int TooManyRequestStatusCode = 429;

        /// <summary>
        /// Provides the ability to interact with the Azure Cosmos DB service.
        /// </summary>
        private readonly IDocumentClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRepository" /> class.
        /// </summary>
        public DocumentRepository(Uri serviceEndpoint, string authKey)
        {
            client = new DocumentClient(serviceEndpoint,
                authKey,
                new JsonSerializerSettings
                {
                    ContractResolver = new CompositeContractResolver
                    {
                        new CamelCasePropertyNamesContractResolver(),
                        new PrivateContractResolver()
                    },
                    Converters = new List<JsonConverter>
                    {
                        new EnumJsonConverter()
                    },
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                });
        }

        /// <summary>
        /// Adds or updates the item.
        /// </summary>
        /// <typeparam name="TEntry">The type of entry being added.</typeparam>
        /// <param name="databaseId">The identifier of the database.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="partitionKey">The key used to partition the collection.</param>
        /// <param name="item">The item to be added.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
        public async Task AddOrUpdateAsync<TEntry>(string databaseId, string collectionId, string partitionKey, TEntry item)
        {
            await AddOrUpdateAsync<TEntry>(databaseId, collectionId, partitionKey, new List<TEntry> { { item } }).ConfigureAwait(false);
        }

        /// <summary>
        /// Adds or updates the collection of items.
        /// </summary>
        /// <typeparam name="TEntry">The type of entry being added.</typeparam>
        /// <param name="databaseId">The identifier of the database.</param>
        /// <param name="collectionId">The identifier of the collection.</param>
        /// <param name="partitionKey">The key used to partition the collection.</param>
        /// <param name="items">The collection of items to be added.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="collectionId"/> is empty or null
        /// or 
        /// <paramref name="databaseId"/> is empty or null
        /// or 
        /// <paramref name="partitionKey"/> is empty or null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="items"/> is null
        /// </exception>
        public async Task AddOrUpdateAsync<TEntry>(string databaseId, string collectionId, string partitionKey, IEnumerable<TEntry> items)
        {
            databaseId.AssertNotNull(nameof(databaseId));
            collectionId.AssertNotNull(nameof(collectionId));
            partitionKey.AssertNotNull(nameof(partitionKey));
            items.AssertNotNull(nameof(items));

            RequestOptions requestOptions = requestOptions = new RequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            };

            foreach (IEnumerable<TEntry> batch in Batch(items, 50))
            {
                await InvokeRequestAsync(() =>
                   client.ExecuteStoredProcedureAsync<int>(
                        UriFactory.CreateStoredProcedureUri(
                        databaseId,
                        collectionId,
                        OperationConstants.BulkImportStoredProcedureName),
                    requestOptions,
                    batch)).ConfigureAwait(false);
            }
        }

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
        /// <exception cref="ArgumentException">
        /// <paramref name="collectionId"/> is empty or null
        /// or 
        /// <paramref name="databaseId"/> is empty or null
        /// or 
        /// <paramref name="partitionKey"/> is empty or null
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="query"/> is null
        /// </exception>
        public async Task<List<TEntry>> QueryAsync<TEntry>(string databaseId, string collectionId, string partitionKey, SqlQuerySpec query, bool crossPartitionQuery)
        {
            databaseId.AssertNotEmpty(nameof(databaseId));
            collectionId.AssertNotEmpty(nameof(collectionId));
            partitionKey.AssertNotEmpty(nameof(partitionKey));
            query.AssertNotNull(nameof(query));

            ResourceResponse<Database> database = await client.CreateDatabaseIfNotExistsAsync(
                new Database
                {
                    Id = databaseId
                }).ConfigureAwait(false);

            ResourceResponse<DocumentCollection> collection = await client.CreateDocumentCollectionIfNotExistsAsync(
                database.Resource.SelfLink,
                new DocumentCollection
                {
                    Id = collectionId,
                    PartitionKey = new PartitionKeyDefinition
                    {
                        Paths = new Collection<string> { partitionKey }
                    }
                });

            FeedOptions options = new FeedOptions
            {
                EnableCrossPartitionQuery = crossPartitionQuery,
                PartitionKey = (!crossPartitionQuery) ? new PartitionKey(partitionKey) : null
            };

            IDocumentQuery<TEntry> documentQuery = client.CreateDocumentQuery<TEntry>(
                collection.Resource.SelfLink,
                query,
                options).AsDocumentQuery();

            List<TEntry> results = new List<TEntry>();

            while (documentQuery.HasMoreResults)
            {
                results.AddRange(await documentQuery.ExecuteNextAsync<TEntry>().ConfigureAwait(false));
            }

            return results;
        }

        private static IEnumerable<IEnumerable<T>> Batch<T>(IEnumerable<T> entities, int batchSize)
        {
            int size = 0;

            while (size < entities.Count())
            {
                yield return entities.Skip(size).Take(batchSize);
                size += batchSize;
            }
        }

        private static async Task<T> InvokeRequestAsync<T>(Func<Task<T>> operation)
        {
            int statusCode;

            try
            {
                return await operation().ConfigureAwait(false);
            }
            catch (DocumentClientException ex)
            {
                statusCode = (int)ex.StatusCode;

                if (statusCode == TooManyRequestStatusCode)
                {
                    // This request can be attempted again after the server specified retry duration. 
                    Thread.Sleep(ex.RetryAfter);

                    return await InvokeRequestAsync(operation).ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}