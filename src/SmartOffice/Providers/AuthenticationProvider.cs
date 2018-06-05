// -----------------------------------------------------------------------
// <copyright file="AuthenticationProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Providers
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Graph;
    using IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// Authentication provider for the Microsoft Graph service client.
    /// </summary>
    /// <seealso cref="IAuthenticationProvider" />
    public sealed class AuthenticationProvider : IAuthenticationProvider
    {
        /// <summary>
        /// Name of the authentication header to be utilized. 
        /// </summary>
        private const string AuthHeaderName = "Authorization";

        /// <summary>
        /// The type of token being utilized for the authentication request.
        /// </summary>
        private const string TokenType = "Bearer";

        private readonly string authority;

        private readonly string clientId;

        private readonly string clientSecret;

        private readonly string customerId;

        public AuthenticationProvider(string authority, string clientId, string clientSecret, string customerId)
        {
            if (string.IsNullOrEmpty(authority))
            {
                throw new ArgumentNullException(nameof(authority));
            }
            else if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            else if (string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }
            else if (string.IsNullOrEmpty(customerId))
            {
                throw new ArgumentNullException(nameof(customerId));
            }

            this.authority = authority;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.customerId = customerId;
        }

        /// <summary>
        /// Performs the necessary authentication and injects the required header.
        /// </summary>
        /// <param name="request">The request being made to Microsoft Graph.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;

            try
            {
                // TODO - Need to make sure that the application of a trailing slash does not break everything.
                authContext = new AuthenticationContext($"{authority}{customerId}");
                authResult = await authContext.AcquireTokenAsync(
                    "https://graph.microsoft.com",
                    new ClientCredential(
                        clientId,
                        clientSecret)).ConfigureAwait(false);

                request.Headers.Add(AuthHeaderName, $"{TokenType} {authResult.AccessToken}");
            }
            finally
            {
                authContext = null;
                authResult = null;
            }
        }
    }
}