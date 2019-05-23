// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.WebJobs.Extensions.SmartOffice
{
    using System;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Description;
    using Graph;
    using Host.Config;
    using IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Smart Office extension configuration where the bindings, collectors, and converters are configured.
    /// </summary>
    [Extension("SmartOffice")]
    internal class SmartOfficeExtensionConfigProvider : IExtensionConfigProvider
    {
        /// <summary>
        /// Used to configure the logging system and create instances of <see cref="ILogger" />.
        /// </summary>
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Used to perform logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartOfficeExtensionConfigProvider" /> class.
        /// </summary>
        /// <param name="loggerFactory">Used to configure the logging system and create instances of <see cref="ILogger" />.</param>
        public SmartOfficeExtensionConfigProvider(ILoggerFactory loggerFactory)
        {
            loggerFactory.AssertNotNull(nameof(loggerFactory));

            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Gets an instance of <see cref="ILogger" /> used to perform logging.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = loggerFactory.CreateLogger("SmartOffice");
                }

                return logger;
            }
        }

        /// <summary>
        /// Performs the operations to initialize the extension.
        /// </summary>
        /// <param name="context">The context for the extension configuration.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="context"/> is null.
        /// </exception>
        public void Initialize(ExtensionConfigContext context)
        {
            context.AssertNotNull(nameof(context));

            context.AddBindingRule<SecureScoreAttribute>().BindToInput(new SecureScoreConverter(this));
        }

        /// <summary>
        /// Gets a configured instance of the <see cref="GraphServiceClient" /> class.
        /// </summary>
        /// <param name="input">The input binding containing the authentication information.</param>
        /// <returns>A configured instance of the <see cref="GraphServiceClient" /> class.</returns>
        public GraphServiceClient GetServiceClient(TokenBaseAttribute input)
        {
            // TODO - Refactor to utilize the new GraphServiceClientFactory.
            return new GraphServiceClient(
                new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    requestMessage
                        .Headers
                        .Authorization = new AuthenticationHeaderValue(
                            "Bearer",
                            await GetTokenAsync(
                                input.ApplicationId,
                                input.ApplicationSecret,
                                input.Resource,
                                input.CustomerId).ConfigureAwait(false));
                }));
        }

        private static async Task<string> GetTokenAsync(string clientId, string clientSecret, string resource, string tenantId)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;

            clientId.AssertNotEmpty(nameof(clientId));
            clientSecret.AssertNotEmpty(nameof(clientSecret));
            resource.AssertNotEmpty(nameof(resource));
            tenantId.AssertNotEmpty(nameof(tenantId));

            authContext = new AuthenticationContext($"https://login.microsoftonline.com/{tenantId}");

            authResult = await authContext.AcquireTokenAsync(
                resource,
                new ClientCredential(
                    clientId,
                    clientSecret)).ConfigureAwait(false);

            return authResult.AccessToken;
        }
    }
}