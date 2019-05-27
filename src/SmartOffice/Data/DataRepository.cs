// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class DataRepository<TEntry> : IDataRepository<TEntry>
    {
        private readonly IDocumentClient client;

        /// <summary>
        /// Identifier of the collection for the repository.
        /// </summary>
        private readonly string collectionId;

        /// <summary>
        /// Identifier of the database forthe  repository.
        /// </summary>
        private readonly string databaseId;

        public DataRepository(Uri serviceEndpoint, string authKey, string databaseId, string collectionId)
        {
            client = new DocumentClient(
                serviceEndpoint,
                authKey,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                },
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });

            this.collectionId = collectionId;
            this.databaseId = databaseId;
        }

        /// <summary>
        /// Deletes the document associated with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier of the document.</param>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        public async Task DeleteAsync(string id)
        {
            await InitializeAsync().ConfigureAwait(false);

            await client.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(databaseId, collectionId, id)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all items available in the repository.
        /// </summary>
        /// <returns>
        /// A collection of items that represent the items in the repository.
        /// </returns>
        public async Task<List<TEntry>> GetAsync()
        {
            FeedResponse<dynamic> response;
            List<TEntry> results = new List<TEntry>();

            try
            {
                string continuation = string.Empty;

                await InitializeAsync().ConfigureAwait(false);

                do
                {
                    response = await client.ReadDocumentFeedAsync(
                        UriFactory.CreateDocumentCollectionUri(databaseId, collectionId),
                        new FeedOptions
                        {
                            MaxItemCount = 100,
                            RequestContinuation = continuation
                        }).ConfigureAwait(false);

                    results.AddRange(response.Select(d => (TEntry)d).ToList());

                    continuation = response.ResponseContinuation;
                } while (!string.IsNullOrEmpty(continuation));

                return results;
            }
            finally
            {
                response = null;
            }
        }

        /// <summary>
        /// Gets an item from the repository.
        /// </summary>
        /// <param name="id">Identifier of the item.</param>
        /// <returns>
        /// The item that matches the specified identifier; if not found null.
        /// </returns>
        public async Task<TEntry> GetAsync(string id)
        {
            Document document;

            try
            {
                await InitializeAsync().ConfigureAwait(false);

                document = await client.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri(databaseId, collectionId, id)).ConfigureAwait(false);

                return (TEntry)(dynamic)document;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }

                return default;
            }
        }

        /// <summary>
        /// Updates an item in the repository.
        /// </summary>
        /// <param name="item">The item to be updated.</param>
        /// <returns>The entry that was updated.</returns>
        public async Task<TEntry> UpdateAsync(TEntry item)
        {
            ResourceResponse<Document> response;

            await InitializeAsync().ConfigureAwait(false);

            response = await client.UpsertDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), item).ConfigureAwait(false);

            return (TEntry)(dynamic)response.Resource;

        }

        private async Task InitializeAsync()
        {
            ResourceResponse<Database> database = await client.CreateDatabaseIfNotExistsAsync(new Database
            {
                Id = databaseId
            }).ConfigureAwait(false);

            await client.CreateDocumentCollectionIfNotExistsAsync(
                database.Resource.SelfLink,
                new DocumentCollection
                {
                    Id = collectionId
                }).ConfigureAwait(false);
        }
    }
}