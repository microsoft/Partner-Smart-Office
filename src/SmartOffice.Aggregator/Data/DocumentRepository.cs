// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    public static class DocumentRepository
    {
        public static async Task<List<TEntity>> GetAsync<TEntity>(IDocumentClient client, string databaseName, string collectionName)
        {
            FeedResponse<dynamic> response;
            List<TEntity> results = new List<TEntity>();

            ResourceResponse<Database> database = await client.CreateDatabaseIfNotExistsAsync(
                new Database
                {
                    Id = databaseName
                }).ConfigureAwait(false);

            ResourceResponse<DocumentCollection> collection = await client.CreateDocumentCollectionIfNotExistsAsync(
                database.Resource.SelfLink,
                new DocumentCollection
                {
                    Id = collectionName
                });

            string continuation = string.Empty;

            do
            {
                response = await client.ReadDocumentFeedAsync(
                    collection.Resource.SelfLink,
                    new FeedOptions
                    {
                        MaxItemCount = 100,
                        RequestContinuation = continuation
                    }).ConfigureAwait(false);

                continuation = response.ResponseContinuation;

                results.AddRange(response.Select(d => (TEntity)d).ToList());
            }
            while (!string.IsNullOrEmpty(continuation));

            return results;
        }
    }
}