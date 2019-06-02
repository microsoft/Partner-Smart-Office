// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;

    public interface IDocumentRepository
    {
        Task AddOrUpdateAsync<TEntry>(string databaseId, string collectionId, string partitionKey, IEnumerable<TEntry> items);

        Task<List<TEntry>> QueryAsync<TEntry>(string databaseName, string collectionName, string partitionKey, SqlQuerySpec querySpec, bool crossPartitionQuery);
    }
}