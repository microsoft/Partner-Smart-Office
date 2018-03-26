// -----------------------------------------------------------------------
// <copyright file="DataRepositoryConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings.Converters
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using Azure.WebJobs;
    using Models;
    using Services;
    using System;

    internal class DataRepositoryConverter : IAsyncConverter<DataRepositoryAttribute, object>
    {
        /// <summary>
        /// Identifier for the bulk import stored procedure.
        /// </summary>
        public const string BulkImportStoredProcedureId = "BulkImport";

        /// <summary>
        /// Name of the secret that contains the Comsos DB access key.
        /// </summary>
        private const string CosmsosDbAccessKey = "CosmosDbAccessKey";

        /// <summary>
        /// Name of the Cosmos DB endpoint setting.
        /// </summary>
        private const string CosmosDbEndpoint = "CosmosDbEndpoint";

        /// <summary>
        /// Identifier for the customers collection.
        /// </summary>
        public const string CustomersCollectionId = "Customers";

        /// <summary>
        /// Identifier for the Azure Cosmos DB database.
        /// </summary>
        public const string DatabaseId = "SmartOffice";

        /// <summary>
        /// Name of the Key Vault endpoint setting.
        /// </summary>
        private const string KeyVaultEndpoint = "KeyVaultEndpoint";

        /// <summary>
        /// Identifier for the secure score collection.
        /// </summary>
        public const string SecureScoreCollectionId = "SecureScore";

        /// <summary>
        /// Identifier for the secure score controls collection.
        /// </summary>
        public const string SecureScoreControlsCollectionId = "SecureScoreControls";

        /// <summary>
        /// Provides access to configuration information for the extension.
        /// </summary>
        private readonly SmartOfficeExtensionConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositoryConverter" /> class.
        /// </summary>
        /// <param name="config">Provides access to configuration information for the extension.</param>
        public DataRepositoryConverter(SmartOfficeExtensionConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<object> ConvertAsync(DataRepositoryAttribute input, CancellationToken cancellationToken)
        {
            KeyVaultService keyVault;
            string authKey;

            try
            {
                keyVault = new KeyVaultService(config.AppSettings.Resolve(KeyVaultEndpoint));

                authKey = await keyVault.GetSecretAsync(CosmsosDbAccessKey).ConfigureAwait(false);

                if (input.DataType == typeof(ControlListEntry))
                {
                    DocumentRepository<ControlListEntry> controls = new DocumentRepository<ControlListEntry>(
                        config.AppSettings.Resolve(CosmosDbEndpoint),
                        authKey,
                        DatabaseId,
                        SecureScoreControlsCollectionId);

                    await controls.InitializeAsync().ConfigureAwait(false);

                    return controls;
                }
                else if (input.DataType == typeof(Customer))
                {
                    DocumentRepository<Customer> customers = new DocumentRepository<Customer>(
                        config.AppSettings.Resolve(CosmosDbEndpoint),
                        authKey,
                        DatabaseId,
                        CustomersCollectionId);

                    await customers.InitializeAsync().ConfigureAwait(false);

                    return customers;
                }
                else if (input.DataType == typeof(SecureScore))
                {
                    DocumentRepository<SecureScore> score = new DocumentRepository<SecureScore>(
                        config.AppSettings.Resolve(CosmosDbEndpoint),
                        authKey,
                        DatabaseId,
                        SecureScoreCollectionId);

                    await score.InitializeAsync().ConfigureAwait(false);

                    return score;
                }

                throw new Exception($"Invalid data type of {input.DataType} specified.");
            }
            finally
            {
                keyVault = null;
            }
        }
    }
}