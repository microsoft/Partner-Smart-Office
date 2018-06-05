// -----------------------------------------------------------------------
// <copyright file="KeyVaultService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Azure.KeyVault;
    using Azure.KeyVault.Models;
    using Newtonsoft.Json;

    public class KeyVaultService : IVaultService, IDisposable
    {
        /// <summary>
        /// Provides the ability to perform HTTP operations. 
        /// </summary>
        private static readonly HttpClient httpClient = new HttpClient();

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
        /// Address for the Azure Key Vault endpoint.
        /// </summary>
        private readonly string endpoint;

        /// <summary>
        /// Provides the ability to perform cryptographic key operations and vault operations 
        /// against the Key Vault service.
        /// </summary>
        private IKeyVaultClient keyVaultClient;

        /// <summary>
        /// Flag indicating whether or not this object has been disposed.
        /// </summary>
        private bool disposed;

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

        private IKeyVaultClient KeyVault => keyVaultClient ?? (keyVaultClient = new KeyVaultClient(GetKeyVaultAccessTokenAsync));

        /// <summary>
        /// Deletes the specified secret from the configured key vault.
        /// </summary>
        /// <param name="secretName">The name of the secret.</param>
        /// <returns>
        /// An instance of the <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task DeleteSecretAsync(string secretName)
        {
            await KeyVault.DeleteSecretAsync(endpoint, secretName).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the secret from the configured key vault.
        /// </summary>
        /// <param name="secretName">The name of the secret.</param>
        /// <returns>The value for the speicifed secret.</returns>
        public async Task<string> GetSecretAsync(string secretName)
        {
            SecretBundle bundle;

            try
            {
                try
                {
                    bundle = await KeyVault.GetSecretAsync(endpoint, secretName).ConfigureAwait(false);
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

        /// <summary>
        /// Sets a secret in the configured key vault. 
        /// </summary>
        /// <param name="secretName">The name of the secret.</param>
        /// <param name="value">The value of the secret.</param>
        /// <param name="contentType">Type of the secret value such as a password.</param>
        /// <returns>
        /// An instance of the <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task SetSecretAsync(string secretName, string value, string contentType)
        {
            await KeyVault.SetSecretAsync(endpoint, secretName, value, null, contentType).ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                KeyVault?.Dispose();
            }

            disposed = true;
        }

        private static async Task<string> GetKeyVaultAccessTokenAsync(string authority, string resource, string scope)
        {
            HttpResponseMessage response = null;
            TokenResponse token;
            string content;
            string endpoint;
            string secret;

            try
            {
                endpoint = Environment.GetEnvironmentVariable(MsiEndpoint);
                secret = Environment.GetEnvironmentVariable(MsiSecret);

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{endpoint}?resource={resource}&api-version=2017-09-01")))
                {
                    request.Headers.Add(SecretHeader, secret);

                    response = await httpClient.SendAsync(request).ConfigureAwait(false);

                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(content);
                    }

                    token = JsonConvert.DeserializeObject<TokenResponse>(content);

                    return token.AccessToken;
                }
            }
            finally
            {
                response?.Dispose();
                token = null;
            }
        }
    }
}