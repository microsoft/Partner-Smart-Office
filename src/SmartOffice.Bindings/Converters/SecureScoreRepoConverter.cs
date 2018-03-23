// -----------------------------------------------------------------------
// <copyright file="SecureScoreRepoConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings.Converters
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Data;
    using Models;
    using Services;

    internal class SecureScoreRepoConverter : IAsyncConverter<SecureScoreRepositoryAttribute, DocumentRepository<SecureScore>>
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
        private SmartOfficeExtensionConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureScoreRepoConverter" /> class.
        /// </summary>
        /// <param name="config">Provides access to configuration information for the extension.</param>
        public SecureScoreRepoConverter(SmartOfficeExtensionConfig config)
        {
            this.config = config;
        }

        public async Task<DocumentRepository<SecureScore>> ConvertAsync(SecureScoreRepositoryAttribute input, CancellationToken cancellationToken)
        {
            DocumentRepository<SecureScore> secureScore;
            KeyVaultService keyVault;
            string authKey;

            try
            {
                keyVault = new KeyVaultService(config.AppSettings.Resolve(KeyVaultEndpoint));

                authKey = await keyVault.GetSecretAsync(CosmsosDbAccessKey).ConfigureAwait(false);

                secureScore = new DocumentRepository<SecureScore>(
                    config.AppSettings.Resolve(CosmosDbEndpoint),
                    authKey,
                    DataConstants.DatabaseId,
                    DataConstants.SecureScoreCollectionId);

                await secureScore.InitializeAsync().ConfigureAwait(false);

                return secureScore;
            }
            finally
            {
                keyVault = null;
            }
        }
    }
}