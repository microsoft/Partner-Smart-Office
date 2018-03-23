// -----------------------------------------------------------------------
// <copyright file="ServiceCredentials.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using IdentityModel.Clients.ActiveDirectory;
    using Rest;

    public sealed class ServiceCredentials : ServiceClientCredentials
    {
        /// <summary>
        /// The authentication scheme utilized by the request.
        /// </summary>
        private const string AuthenticationScheme = "Bearer";

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceCredentials" /> class.
        /// </summary>
        /// <param name="clientId">
        /// The client identifer to be used when requesting an access token.
        /// </param>
        /// <param name="clientSecret">
        /// The client secret to be used when requesting an access token.
        /// </param>
        /// <param name="resource">
        /// Identifier of the target resource that is the recipient of the requested token.
        /// </param>
        /// <param name="tenantId">
        /// The tenant identifier to be used when requesting an access token.
        /// </param>
        public ServiceCredentials(string clientId, string clientSecret, string resource, string tenantId)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Resource = resource;
            TenantId = tenantId;
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the identity of the target resource that is the recipient of the requested token.
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Applies the credentials to the HTTP request.
        /// </summary>
        /// <param name="request">
        /// The HTTP request message
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token to monitor.
        /// </param>
        /// <returns>
        /// An instance of the <see cref="Task"/> class that represents the asynchronous operation.
        /// </returns>
        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;

            try
            {
                authContext = new AuthenticationContext($"https://login.microsoftonline.com/{TenantId}");

                authResult = await authContext.AcquireTokenAsync(
                    Resource,
                    new ClientCredential(
                        ClientId,
                        ClientSecret)).ConfigureAwait(false);

                request.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme, authResult.AccessToken);
            }
            finally
            {
                authContext = null;
                authResult = null;
            }
        }

        public async Task<string> GetTokenAsync()
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;

            try
            {
                authContext = new AuthenticationContext($"https://login.microsoftonline.com/{TenantId}");

                authResult = await authContext.AcquireTokenAsync(
                    Resource,
                    new ClientCredential(
                        ClientId,
                        ClientSecret)).ConfigureAwait(false);

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