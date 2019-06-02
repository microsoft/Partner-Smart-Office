// Copyright (c) Microsoft. All rights reserved.
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

    public static class DocumentRepository
    {
        /// <summary>
        /// HTTP status code returned when the collection has exceeded the provisioned
        /// throughput limit. The request should be retried after the server specified 
        /// retry after duration.
        /// </summary>
        private const int TooManyRequestStatusCode = 429;

        /// <summary>
        /// Add or update the collection of items in the repository.
        /// </summary>
        /// <param name="items">A collection of items to be added or updated.</param>
        /// <param name="partitionKey">Key used to partition the data.</param>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        public static async Task AddOrUpdateAsync<TEntry>(IDocumentClient client, string databaseId, string collectionId, string partitionKey, IEnumerable<TEntry> items)
        {
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
                        Constants.BulkImportStoredProcedureName),
                    requestOptions,
                    batch)).ConfigureAwait(false);
            }
        }

        public static async Task<List<TEntry>> QueryAsync<TEntry>(IDocumentClient client, string databaseName, string collectionName, string partitionKey, SqlQuerySpec querySpec, bool crossPartitionQuery)
        {
            ResourceResponse<Database> database = await client.CreateDatabaseIfNotExistsAsync(
                new Database
                {
                    Id = databaseName
                }).ConfigureAwait(false);

            ResourceResponse<DocumentCollection> collection = await client.CreateDocumentCollectionIfNotExistsAsync(
                database.Resource.SelfLink,
                new DocumentCollection
                {
                    Id = collectionName,
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

            IDocumentQuery<TEntry> query = client.CreateDocumentQuery<TEntry>(
                collection.Resource.SelfLink,
                querySpec,
                options).AsDocumentQuery();

            List<TEntry> results = new List<TEntry>();

            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<TEntry>().ConfigureAwait(false));
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

        /// <summary>
        /// Invokes the specified request and retries if necessary.
        /// </summary>
        /// <typeparam name="T">The type of object to be returned.</typeparam>
        /// <param name="operation">The operation to be invoked.</param>
        /// <returns>The results from the invoked operation.</returns>
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