// -----------------------------------------------------------------------
// <copyright file="SmartOfficeExtensionConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings
{
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host;
    using Azure.WebJobs.Host.Config;
    using Converters;
    using Data;
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
        /// Used to write to the function application log.
        /// </summary>
        private TraceWriter log;

        /// <summary>
        /// Gets the reference to the application settings resolver.
        /// </summary>
        internal INameResolver AppSettings { get; private set; }

        /// <summary>
        /// Initialize the binding extension
        /// </summary>
        /// <param name="context">Context for the extension</param>
        public void Initialize(ExtensionConfigContext context)
        {
            AppSettings = context.Config.NameResolver;
            log = context.Trace;

            context.AddBindingRule<CustomersRepositoryAttribute>().BindToInput(new CustomersRepoConverter(this));
            context.AddBindingRule<SecureScoreAttribute>().BindToInput(new SecureScoreConverter(this));
            context.AddBindingRule<SecureScoreRepositoryAttribute>().BindToInput(new SecureScoreRepoConverter(this));
            context.AddBindingRule<StorageServiceAttribute>().BindToInput(new StorageServiceConverter(this));
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
                keyVault = new KeyVaultService(AppSettings.Resolve(KeyVaultEndpoint));

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
                keyVault = null;
            }
        }
    }
}