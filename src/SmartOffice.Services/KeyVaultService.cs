// -----------------------------------------------------------------------
// <copyright file="KeyVaultService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.KeyVault;
    using Azure.KeyVault.Models;

    public class KeyVaultService : IVaultSerivce
    {
        /// <summary>
        /// Name of the MSI endpoint environment variable.
        /// </summary>
        private const string MsiEndpoint = "MSI_ENDPOINT";

        /// <summary>
        /// Name of the MSI secret environment variable.
        /// </summary>
        private const string MsiSecret = "MSI_SECRET";

        /// <summary>
        /// Name of the Secret HTTP header.
        /// </summary>
        private const string SecretHeader = "Secret";

        /// <summary>
        /// Error code returned when a secret is not found in the vault.
        /// </summary>
        private const string NotFoundErrorCode = "SecretNotFound";

        /// <summary>
        /// Provides the ability to perform cryptographic key operations and vault operations 
        /// against the Key Vault service.
        /// </summary>
        private IKeyVaultClient keyVaultClient;

        /// <summary>
        /// Address for the Azure Key Vault endpoint.
        /// </summary>
        private string endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultService"/> class.
        /// </summary>
        /// <param name="endpoint">Address for the Azure Key Vault endpoint.</param>
        public KeyVaultService(string endpoint)
        {
            this.endpoint = endpoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultService" /> class.
        /// </summary>
        /// <param name="keyVaultClient">
        /// Provides the ability to perform cryptographic key operations and vault operations 
        /// against the Key Vault service.
        /// </param>
        /// <param name="endpoint">The Azure Key Vault service endpoint.</param>
        public KeyVaultService(IKeyVaultClient keyVaultClient, string endpoint)
        {
            this.endpoint = endpoint;
            this.keyVaultClient = keyVaultClient;
        }

        private IKeyVaultClient KeyVault
        {
            get
            {
                if (keyVaultClient == null)
                {
                    keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetKeyVaultAccessTokenAsync));
                }

                return keyVaultClient;
            }
        }

        /// <summary>
        /// Gets the secret value from the configured instance of Azure Key Vault.
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be retrieved.</param>
        /// <returns>The value for the speicifed secret.</returns>
        public async Task<string> GetSecretAsync(string identifier)
        {
            SecretBundle bundle;

            try
            {
                try
                {
                    bundle = await KeyVault.GetSecretAsync(endpoint, identifier).ConfigureAwait(false);
                }
                catch (KeyVaultErrorException ex)
                {
                    if (ex.Body.Error.Code.Equals(NotFoundErrorCode, StringComparison.CurrentCultureIgnoreCase))
                    {
                        bundle = null;
                    }
                    else
                    {
                        throw;
                    }
                }

                return bundle?.Value;
            }
            finally
            {
                bundle = null;
            }
        }

        private static async Task<string> GetKeyVaultAccessTokenAsync(string authority, string resource, string scope)
        {
            TokenResponse response;
            string endpoint;
            string secret;

            try
            {
                endpoint = Environment.GetEnvironmentVariable(MsiEndpoint);
                secret = Environment.GetEnvironmentVariable(MsiSecret);

                response = await HttpService.Instance.GetAsync<TokenResponse>(
                     new Uri($"{endpoint}?resource={resource}&api-version=2017-09-01"),
                     new Dictionary<string, string> { { SecretHeader, secret } }).ConfigureAwait(false);

                return response.AccessToken;
            }
            finally
            {
                response = null;
            }
        }
    }
}