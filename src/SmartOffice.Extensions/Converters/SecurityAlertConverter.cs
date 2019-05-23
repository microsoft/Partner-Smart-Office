// -----------------------------------------------------------------------
// <copyright file="SecurityAlertConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions.Converters
{
    using System.Collections.Generic;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Graph;
    using IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Services.KeyVault;

    public class SecurityAlertConverter : IAsyncConverter<SecurityAlertsAttribute, List<Alert>>
    {
        private readonly ILogger log;

        private readonly IVaultService vault;

        private readonly SmartOfficeOptions options;

        public SecurityAlertConverter(ILoggerFactory loggerFactory, IOptions<SmartOfficeOptions> options, IVaultService vault)
        {
            log = loggerFactory?.CreateLogger("Host.Bindings.SecurityAlertConverter");
            this.options = options.Value;
            this.vault = vault;
        }

        public async Task<List<Alert>> ConvertAsync(SecurityAlertsAttribute input, CancellationToken cancellationToken)
        {
            GraphServiceClient client;
            ISecurityAlertsCollectionPage page;
            List<Alert> alerts;

            try
            {
                client = new GraphServiceClient(
                    new DelegateAuthenticationProvider(async (requestMessage) =>
                    {
                        requestMessage
                            .Headers
                            .Authorization = new AuthenticationHeaderValue(
                                "Bearer",
                                await GetTokenAsync(
                                    input.ApplicationId,
                                    await vault.GetSecretAsync(options.KeyVaultEndpoint, input.SecretName).ConfigureAwait(false),
                                    input.Resource,
                                    input.CustomerId).ConfigureAwait(false));
                    }));

                page = await client.Security.Alerts.Request().GetAsync().ConfigureAwait(false);

                alerts = new List<Alert>(page.CurrentPage);

                while (page.NextPageRequest != null)
                {
                    page = await page.NextPageRequest.GetAsync().ConfigureAwait(false);
                    alerts.AddRange(page.CurrentPage);
                }

                return alerts;
            }
            catch (ClientException ex)
            {
                log.LogError(ex, $"Encountered an error when processing {input.CustomerId}");
                return null;
            }
        }

        private static async Task<string> GetTokenAsync(string clientId, string clientSecret, string resource, string tenantId)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;

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