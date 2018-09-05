// -----------------------------------------------------------------------
// <copyright file="KeyVaultService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.KeyVault
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.KeyVault;
    using Azure.KeyVault.Models;
    using Azure.Services.AppAuthentication;

    public class KeyVaultService : IVaultService, IDisposable
    {
        /// <summary>
        /// Provides a collection of secret values.
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> secrets = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Provides the ability to perform HTTP operations. 
        /// </summary>
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Used to help ensure that data secrets are initialized in a thread safe manner.
        /// </summary>
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

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
        /// Flag indicating whether or not this object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultService"/> class.
        /// </summary>
        public KeyVaultService()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultService" /> class.
        /// </summary>
        /// <param name="keyVaultClient">
        /// Provides the ability to perform cryptographic key operations and vault operations 
        /// against the Key Vault service.
        /// </param>
        /// <param name="endpoint">The Azure Key Vault service endpoint.</param>
        public KeyVaultService(IKeyVaultClient keyVaultClient)
        {
            this.keyVaultClient = keyVaultClient;
        }

        private IKeyVaultClient KeyVault => keyVaultClient ??
            (keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    new AzureServiceTokenProvider().KeyVaultTokenCallback),
                httpClient));

        /// <summary>
        /// Deletes the specified secret from the configured key vault.
        /// </summary>
        /// <param name="endpoint">Address for the Azure Key Vault endpoint.</param>
        /// <param name="secretName">The name of the secret.</param>
        /// <returns>
        /// An instance of the <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task DeleteSecretAsync(string endpoint, string secretName)
        {
            await KeyVault.DeleteSecretAsync(endpoint, secretName).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the secret from the configured key vault.
        /// </summary>
        /// <param name="endpoint">Address for the Azure Key Vault endpoint.</param>
        /// <param name="secretName">The name of the secret.</param>
        /// <returns>The value for the speicifed secret.</returns>
        public async Task<string> GetSecretAsync(string endpoint, string secretName)
        {
            SecretBundle bundle;

            try
            {
                await semaphore.WaitAsync().ConfigureAwait(false);

                if (!secrets.ContainsKey(secretName))
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

                    secrets[secretName] = bundle?.Value;
                }

                return secrets[secretName];
            }
            finally
            {
                bundle = null;

                semaphore.Release();
            }
        }

        /// <summary>
        /// Sets a secret in the configured key vault. 
        /// </summary>
        /// <param name="endpoint">Address for the Azure Key Vault endpoint.</param>
        /// <param name="secretName">The name of the secret.</param>
        /// <param name="value">The value of the secret.</param>
        /// <param name="contentType">Type of the secret value such as a password.</param>
        /// <returns>
        /// An instance of the <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        public async Task SetSecretAsync(string endpoint, string secretName, string value, string contentType)
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
    }
}