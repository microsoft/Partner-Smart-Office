// -----------------------------------------------------------------------
// <copyright file="TokenConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings.Converters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.KeyVault;
    using Azure.KeyVault.Models;
    using Azure.Services.AppAuthentication;
    using Azure.WebJobs;
    using IdentityModel.Clients.ActiveDirectory;

    public class TokenConverter : IAsyncConverter<TokenAttribute, string>
    {
        /// <summary>
        /// Provides access to application settings.
        /// </summary>
        private INameResolver appSettings;

        /// <summary>
        /// Error code returned when a secret is not found in the vault.
        /// </summary>
        private const string NotFoundErrorCode = "SecretNotFound";

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenConverter" /> class.
        /// </summary>
        /// <param name="appSettings">Provides access to application settings.</param>
        public TokenConverter(INameResolver appSettings)
        {
            this.appSettings = appSettings;
        }

        public async Task<string> GetSecretAsync(string identifier)
        {
            AzureServiceTokenProvider tokenProvider;
            SecretBundle bundle;

            try
            {
                tokenProvider = new AzureServiceTokenProvider();

                using (KeyVaultClient client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(tokenProvider.KeyVaultTokenCallback)))
                {
                    try
                    {
                        bundle = await client.GetSecretAsync(appSettings.Resolve("KeyVaultEndpoint"), identifier).ConfigureAwait(false);
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
                }

                return bundle?.Value;
            }
            finally
            {
                bundle = null;
                tokenProvider = null;
            }
        }

        public async Task<string> ConvertAsync(TokenAttribute input, CancellationToken cancellationToken)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;
            string appSecret;

            try
            {
                authContext = new AuthenticationContext($"https://login.microsoftonline.com/common");

                appSecret = await GetSecretAsync(input.AppSecretId).ConfigureAwait(false);

                authResult = await authContext.AcquireTokenAsync(
                    input.Resource,
                    new ClientCredential(
                        appSettings.Resolve(input.ApplicationId),
                        appSecret)).ConfigureAwait(false);

                return authResult.AccessToken;
            }
            finally
            {
                authContext = null;
                authResult = null;
            }
        }
    }
}