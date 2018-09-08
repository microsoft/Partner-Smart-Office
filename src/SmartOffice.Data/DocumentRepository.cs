// -----------------------------------------------------------------------
// <copyright file="DocumentRepository.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Documents;
    using Azure.Documents.Client;
    using Azure.Documents.Linq;
    using Models.PartnerCenter.JsonConverters;
    using Newtonsoft.Json;

    public class DocumentRepository<TEntity> : IDocumentRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Path for the BulkImport.js embedded resource.
        /// </summary>
        private const string BulkImportEmbeddedResource = "Microsoft.Partner.SmartOffice.Data.Scripts.BulkImport.js";

        /// <summary>
        /// Name of the bulk import stored procedure.
        /// </summary>
        private const string BulkImportStoredProcId = "BulkImport";

        /// <summary>
        /// HTTP status code returned when the collection has exceeded the provisioned
        /// throughput limit. The request should be retried after the server specified 
        /// retry after duration.
        /// </summary>
        private const int TooManyRequestStatusCode = 429;

        /// <summary>
        /// Access key used for authentication purposes.
        /// </summary>
        private readonly string authKey;

        /// <summary>
        /// Identifier of the collection for the repository.
        /// </summary>
        private readonly string collectionId;

        /// <summary>
        /// Identifier of the database forthe  repository.
        /// </summary>
        private readonly string databaseId;

        /// <summary>
        /// Path to the key used to partition the data.
        /// </summary>
        private readonly string partitionKeyPath;

        /// <summary>
        /// Endpoint address for the instance of Cosmos DB.
        /// </summary>
        private readonly string serviceEndpoint;

        /// <summary>
        /// Provides the ability to interact with Cosmos DB.
        /// </summary>
        private static IDocumentClient documentClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="serviceEndpoint">Service address for the instance of Cosmos DB.</param>
        /// <param name="authKey">Access key used for authentication purposes.</param>
        /// <param name="databaseId">Identifier of the databse for this repository.</param>
        /// <param name="collectionId">Identifier of the collection for this repository.</param>
        /// <param name="partitionKeyPath">Path to the key used to partition the data.</param>
        public DocumentRepository(string serviceEndpoint, string authKey, string databaseId, string collectionId, string partitionKeyPath = null)
        {
            this.authKey = authKey;
            this.collectionId = collectionId;
            this.databaseId = databaseId;
            this.partitionKeyPath = partitionKeyPath;
            this.serviceEndpoint = serviceEndpoint;
        }

        /// <summary>
        /// Initializes a new instance fo the <see cref="DocumentRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="client">Client used to perform operations against Cosmos DB.</param>
        /// <param name="databaseId">Identifier of the databse for this repository.</param>
        /// <param name="collectionId">Identifier of the collection for this repository.</param>
        /// <param name="partitionKeyPath">Path to the key used to partition the data.</param>
        public DocumentRepository(IDocumentClient client, string databaseId, string collectionId, string partitionKeyPath = null)
        {
            documentClient = client;
            this.collectionId = collectionId;
            this.databaseId = databaseId;
            this.partitionKeyPath = partitionKeyPath;
        }

        /// <summary>
        /// Provides the ability to interact with Cosmos DB.
        /// </summary>
        private IDocumentClient Client => documentClient ?? (documentClient = new DocumentClient(
                                              new Uri(serviceEndpoint),
                                              authKey,
                                              new JsonSerializerSettings
                                              {
                                                  Converters = new List<JsonConverter>
                                                  {
                                                      {
                                                          new EnumJsonConverter()
                                                      }
                                                  },
                                                  DateFormatHandling = DateFormatHandling.IsoDateFormat,
                                                  DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                                                  NullValueHandling = NullValueHandling.Ignore,
                                                  ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                                              },
                                              new ConnectionPolicy
                                              {
                                                  ConnectionMode = ConnectionMode.Direct,
                                                  ConnectionProtocol = Protocol.Tcp
                                              }));

        /// <summary>
        /// Add or update an item in the repository.
        /// </summary>
        /// <param name="item">The item to be added or updated.</param>
        /// <param name="partitionKey">Key used to partition the data.</param>
        /// <returns>The entity that was added or updated.</returns>
        public async Task<TEntity> AddOrUpdateAsync(TEntity item, string partitionKey = null)
        {
            RequestOptions requestOptions;
            ResourceResponse<Document> response;

            try
            {
                requestOptions = new RequestOptions(); 

                if (!string.IsNullOrEmpty(partitionKeyPath))
                {
                    requestOptions.PartitionKey = new PartitionKey(partitionKey);
                }

                response = await Client.UpsertDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId),
                    item,
                    requestOptions).ConfigureAwait(false);

                return (TEntity)(dynamic)response.Resource;
            }
            finally
            {
                requestOptions = null; 
                response = null;
            }
        }

        /// <summary>
        /// Add or update the collection of items in the repository.
        /// </summary>
        /// <param name="items">A collection of items to be added or updated.</param>
        /// <param name="partitionKey">Key used to partition the data.</param>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        public async Task AddOrUpdateAsync(IEnumerable<TEntity> items, string partitionKey = null)
        {
            RequestOptions requestOptions;
            bool resetThroughput = false;

            try
            {
                if (items.Count() > 1000 && items.Count() < 2000)
                {
                    resetThroughput = true;
                    await UpdateOfferAsync(1000).ConfigureAwait(false);
                }
                else if (items.Count() > 2000)
                {
                    resetThroughput = true;
                    await UpdateOfferAsync(2000).ConfigureAwait(false);
                }

                requestOptions = new RequestOptions();

                if (!string.IsNullOrEmpty(partitionKeyPath))
                {
                    requestOptions.PartitionKey = new PartitionKey(partitionKey);
                }

                foreach (IEnumerable<TEntity> batch in Batch(items, 500))
                {
                    await InvokeRequestAsync(() =>
                        Client.ExecuteStoredProcedureAsync<int>(
                            UriFactory.CreateStoredProcedureUri(
                            databaseId,
                            collectionId,
                            BulkImportStoredProcId),
                        requestOptions,
                        batch)).ConfigureAwait(false);
                }

                if (resetThroughput == true)
                {
                    await UpdateOfferAsync(1000).ConfigureAwait(false);
                }
            }
            finally
            {
                requestOptions = null;
            }
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
            await Client.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(databaseId, collectionId, id)).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets an item from the repository.
        /// </summary>
        /// <param name="id">Identifier of the item.</param>
        /// <returns>
        /// The item that matches the specified identifier; if not found null.
        /// </returns>
        public async Task<TEntity> GetAsync(string id)
        {
            Document document;

            try
            {
                document = await Client.ReadDocumentAsync(
                    UriFactory.CreateDocumentUri(databaseId, collectionId, id)).ConfigureAwait(false);

                return (TEntity)(dynamic)document;
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }

                return null;
            }
            finally
            {
                document = null;
            }
        }

        /// <summary>
        /// Gets all items available in the repository.
        /// </summary>
        /// <param name="partitionKey">Key used to partition the data.</param>
        /// <returns>
        /// A collection of items that represent the items in the repository.
        /// </returns>
        public async Task<List<TEntity>> GetAsync()
        {
            FeedResponse<dynamic> response;
            List<TEntity> results;

            try
            {
                response = await Client.ReadDocumentFeedAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId)).ConfigureAwait(false);

                results = response.Select(d => (TEntity)d).ToList();

                return results;
            }
            finally
            {
                response = null;
            }
        }

        /// <summary>
        /// Gets a sequence of items for the repository that matches the query. 
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="partitionKey">Key used to partition the data.</param>
        /// <returns>
        /// A collection that contains items from the repository that satisfy the condition specified by predicate.
        /// </returns>
        public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, string partitionKey)
        {
            IDocumentQuery<TEntity> query;
            List<TEntity> results;

            try
            {

                query = Client.CreateDocumentQuery<TEntity>(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId),
                    new FeedOptions
                    {
                        PartitionKey = new PartitionKey(partitionKey)
                    })
                    .Where(predicate)
                    .AsDocumentQuery();

                results = new List<TEntity>();

                while (query.HasMoreResults)
                {
                    results.AddRange(await query.ExecuteNextAsync<TEntity>().ConfigureAwait(false));
                }

                return results;
            }
            finally
            {
                query = null;
            }
        }

        /// <summary>
        /// Performs the initialization operations for the repository.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        public async Task InitializeAsync()
        {
            await CreateDatabaseIfNotExistsAsync().ConfigureAwait(false);
            await CreateCollectionIfNotExistsAsync().ConfigureAwait(false);
            await CreateStoredProcedureIfNotExistsAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Updates an item in the repository.
        /// </summary>
        /// <param name="item">The item to be updated.</param>
        /// <returns>The entity that updated.</returns>
        public async Task<TEntity> UpdateAsync(TEntity item)
        {
            ResourceResponse<Document> response;

            try
            {
                response = await Client.UpsertDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), item).ConfigureAwait(false);

                return (TEntity)(dynamic)response.Resource;
            }
            finally
            {
                response = null;
            }
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
        /// Creates the collection if it does not already exists.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        private async Task CreateCollectionIfNotExistsAsync()
        {
            DocumentCollection collection;

            try
            {
                await Client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId)).ConfigureAwait(false);
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }

                collection = new DocumentCollection
                {
                    Id = collectionId
                };

                if (!string.IsNullOrEmpty(partitionKeyPath))
                {
                    collection.PartitionKey.Paths.Add(partitionKeyPath);
                }

                await Client.CreateDocumentCollectionAsync(
                    UriFactory.CreateDatabaseUri(databaseId),
                    collection,
                    new RequestOptions { OfferThroughput = 500 }).ConfigureAwait(false);
            }
            finally
            { }
        }

        /// <summary>
        /// Creates the database if it does not already exists.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId)).ConfigureAwait(false);
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }

                await Client.CreateDatabaseAsync(
                    new Database { Id = databaseId }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates the stored procedure if it does not already exists.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        private async Task CreateStoredProcedureIfNotExistsAsync()
        {
            string storedProc;

            try
            {
                await Client.ReadStoredProcedureAsync(
                    UriFactory.CreateStoredProcedureUri(
                        databaseId,
                        collectionId,
                        BulkImportStoredProcId)).ConfigureAwait(false);
            }
            catch (DocumentClientException ex)
            {
                if (ex.StatusCode != HttpStatusCode.NotFound)
                {
                    throw;
                }

                using (StreamReader reader = new StreamReader(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(BulkImportEmbeddedResource)))
                {
                    storedProc = await reader.ReadToEndAsync().ConfigureAwait(false);
                }

                await Client.CreateStoredProcedureAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId),
                    new StoredProcedure
                    {
                        Body = storedProc,
                        Id = BulkImportStoredProcId
                    }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Invokes the specified request and retries if necessary.
        /// </summary>
        /// <typeparam name="T">The type of object to be returned.</typeparam>
        /// <param name="operation">The operation to be invoked.</param>
        /// <returns>The results from the invoked operation.</returns>
        private async Task<T> InvokeRequestAsync<T>(Func<Task<T>> operation)
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

                throw;
            }
        }

        /// <summary>
        /// Updates the throughput for the specified collection.
        /// </summary>
        /// <param name="offerThroughput">The provisioned throughtput for this offer.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
        private async Task UpdateOfferAsync(int offerThroughput)
        {
            DocumentCollection collection;
            Offer offer;

            try
            {
                collection = await Client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseId, collectionId)).ConfigureAwait(false);

                offer = Client.CreateOfferQuery()
                    .Where(o => o.ResourceLink == collection.SelfLink)
                    .AsEnumerable()
                    .SingleOrDefault();

                offer = new OfferV2(offer, offerThroughput);

                await Client.ReplaceOfferAsync(offer).ConfigureAwait(false);
            }
            finally
            {
                collection = null;
                offer = null;
            }
        }
    }
}