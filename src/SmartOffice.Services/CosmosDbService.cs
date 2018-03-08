// -----------------------------------------------------------------------
// <copyright file="CosmosDbService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using Azure.Documents;
    using Azure.Documents.Client;

    public class CosmosDbService : ICosmosDbService
    {
        /// <summary>
        /// Path for the BulkImport.js embedded resource.
        /// </summary>
        private const string BulkImportEmbeddedResource = "Microsoft.Partner.SmartOffice.Services.Scripts.BulkImport.js";

        /// <summary>
        /// Provides the ability to interact with Azure Cosmos DB.
        /// </summary>
        private DocumentClient client;

        /// <summary>
        /// Provides the ability to interact with Azure Key Vault.
        /// </summary>
        private IVaultSerivce vault;

        /// <summary>
        /// Singleton instance of the <see cref="CosmosDbService" /> class.
        /// </summary>
        private static Lazy<CosmosDbService> instance = new Lazy<CosmosDbService>(() => new CosmosDbService());

        /// <summary>
        /// Initialize a new instance of the <see cref="CosmosDbService" /> class.
        /// </summary>
        public CosmosDbService()
        {
        }

        /// <summary>
        /// Gets an instance of the <see cref="CosmosDbService" /> class.
        /// </summary>
        public static CosmosDbService Instance => instance.Value;

        public async Task InitializeAsync(CosmosDbOptions options)
        {
            vault = new KeyVaultService(options.KeyVaultEndpoint);

            client = new DocumentClient(
                new Uri(options.Endpoint),
                await vault.GetSecretAsync(options.AccessKeySecretName).ConfigureAwait(false),
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });

            await CreateResourcesAsync(options).ConfigureAwait(false);
        }

        public async Task ExecuteStoredProcedureAsync(string databaseId, string collectionId, string storedProcedureId, params dynamic[] procedureParams)
        {
            await client.ExecuteStoredProcedureAsync<int>(
                UriFactory.CreateStoredProcedureUri(
                    databaseId,
                    collectionId,
                    storedProcedureId),
                procedureParams).ConfigureAwait(false);
        }

        private async Task CreateResourcesAsync(CosmosDbOptions options)
        {
            bool createStoredProc;
            string storedProcBody;

            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = options.DatabaseId }).ConfigureAwait(false);

            using (StreamReader reader = new StreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(BulkImportEmbeddedResource)))
            {
                storedProcBody = await reader.ReadToEndAsync().ConfigureAwait(false);
            }

            foreach (string collection in options.Collections)
            {
                await client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(options.DatabaseId),
                    new DocumentCollection { Id = collection },
                    new RequestOptions { OfferThroughput = 400 }).ConfigureAwait(false);

                try
                {
                    createStoredProc = false;

                    await client.ReadStoredProcedureAsync(
                        UriFactory.CreateStoredProcedureUri(
                            options.DatabaseId,
                            collection,
                            options.BulkImportStoredProcedureId)).ConfigureAwait(false);
                }
                catch (DocumentClientException ex)
                {
                    if (ex.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw;
                    }

                    createStoredProc = true;
                }

                if (createStoredProc)
                {
                    await client.CreateStoredProcedureAsync(
                        UriFactory.CreateDocumentCollectionUri(DataConstants.DatabaseId, collection),
                        new StoredProcedure
                        {
                            Body = storedProcBody,
                            Id = options.BulkImportStoredProcedureId
                        }).ConfigureAwait(false);
                }
            }

        }
    }
}