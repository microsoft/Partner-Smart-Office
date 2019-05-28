// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    public static class DocumentRepository
    {
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
    }
}