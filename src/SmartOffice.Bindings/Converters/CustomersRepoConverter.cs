// -----------------------------------------------------------------------
// <copyright file="CustomersRepoConverter.cs" company="Microsoft">
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

    internal class CustomersRepoConverter : IAsyncConverter<CustomersRepositoryAttribute, DocumentRepository<Customer>>
    {
        /// <summary>
        /// Name of the secret that contains the Comsos DB access key.
        /// </summary>
        private const string CosmsosDbAccessKey = "CosmosDbAccessKey";

        /// <summary>
        /// Name of the Cosmos DB endpoint setting.
        /// </summary>
        private const string CosmosDbEndpoint = "CosmosDbEndpoint";

        /// <summary>
        /// Name of the Key Vault endpoint setting.
        /// </summary>
        private const string KeyVaultEndpoint = "KeyVaultEndpoint";

        /// <summary>
        /// Provides access to configuration information for the extension.
        /// </summary>
        private readonly SmartOfficeExtensionConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersRepoConverter" /> class.
        /// </summary>
        /// <param name="config">Provides access to configuration information for the extension.</param>
        public CustomersRepoConverter(SmartOfficeExtensionConfig config)
        {
            this.config = config;
        }

        public async Task<DocumentRepository<Customer>> ConvertAsync(CustomersRepositoryAttribute input, CancellationToken cancellationToken)
        {
            DocumentRepository<Customer> customers;
            KeyVaultService keyVault;
            string authKey;

            try
            {
                keyVault = new KeyVaultService(config.AppSettings.Resolve(KeyVaultEndpoint));

                authKey = await keyVault.GetSecretAsync(CosmsosDbAccessKey).ConfigureAwait(false);

                customers = new DocumentRepository<Customer>(
                    config.AppSettings.Resolve(CosmosDbEndpoint),
                    authKey,
                    DataConstants.DatabaseId,
                    DataConstants.CustomersCollectionId);

                await customers.InitializeAsync().ConfigureAwait(false);

                return customers;
            }
            finally
            {
                keyVault = null;
            }
        }
    }
}
