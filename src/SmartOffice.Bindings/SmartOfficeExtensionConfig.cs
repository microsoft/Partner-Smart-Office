// -----------------------------------------------------------------------
// <copyright file="SmartOfficeExtensionConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host;
    using Azure.WebJobs.Host.Config;
    using Collectors;
    using Converters;
    using IdentityModel.Clients.ActiveDirectory;
    using Models;
    using Services;

    public class SmartOfficeExtensionConfig : IExtensionConfigProvider
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
        /// Name of the Azure Storage account connection string secret.
        /// </summary>
        private const string StorageConnectionStringSecret = "StorageConnectionString";

        /// <summary>
        /// Used to access application settings for the function application.
        /// </summary>
        private INameResolver appSettings;

        /// <summary>
        /// Used to write to the function application log.
        /// </summary>
        private TraceWriter log;

        /// <summary>
        /// Initialize the binding extension
        /// </summary>
        /// <param name="context">Context for the extension</param>
        public void Initialize(ExtensionConfigContext context)
        {
            appSettings = context.Config.NameResolver;
            log = context.Trace;

            Task.Run(() => InitializeAsync()).Wait();

            context.AddBindingRule<CustomersAttribute>().BindToCollector(CreateCollector);
            context.AddBindingRule<SecureScoreAttribute>().BindToInput(new SecureScoreConverter(this));
            context.AddBindingRule<TokenAttribute>().BindToInput(new TokenConverter(this));
        }

        /// <summary>
        /// Gets an OAuth access token for the specified resource.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="clientId">Identifier of the client requesting the token.</param>
        /// <param name="secretName">Name of the secret that contains the secret of the client requesting the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <returns>A string that represents the requested OAuth access token.</returns>
        public async Task<string> GetAccessTokenAsync(string authority, string clientId, string secretName, string resource)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;
            KeyVaultService keyVault;
            string clientSecret;

            try
            {
                keyVault = new KeyVaultService(appSettings.Resolve(KeyVaultEndpoint));

                clientSecret = await keyVault.GetSecretAsync(secretName).ConfigureAwait(false);

                authContext = new AuthenticationContext(authority);
                authResult = await authContext.AcquireTokenAsync(
                    resource,
                    new ClientCredential(
                        clientId,
                        clientSecret)).ConfigureAwait(false);

                return authResult.AccessToken;
            }
            finally
            {
                authContext = null;
                authResult = null;
            }
        }

        private static IAsyncCollector<List<Customer>> CreateCollector(CustomersAttribute attribute)
        {
            return new CustomersAsyncCollector(attribute);
        }

        private async Task InitializeAsync()
        {
            await CosmosDbService.Instance.InitializeAsync(
                new CosmosDbOptions
                {
                    BulkImportStoredProcedureId = DataConstants.BulkImportStoredProcedureId,
                    Collections = new List<string>
                    {
                       DataConstants.CustomersCollectionId,
                       DataConstants.SecureScoreCollectionId
                    },
                    DatabaseId = DataConstants.DatabaseId,
                    Endpoint = appSettings.Resolve(CosmosDbEndpoint),
                    KeyVaultEndpoint = appSettings.Resolve(KeyVaultEndpoint),
                    AccessKeySecretName = CosmsosDbAccessKey
                }).ConfigureAwait(false);

            await StorageService.Instance.InitializeAsync(
                appSettings.Resolve(KeyVaultEndpoint),
                StorageConnectionStringSecret).ConfigureAwait(false);
        }
    }
}