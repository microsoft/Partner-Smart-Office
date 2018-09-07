// -----------------------------------------------------------------------
// <copyright file="SecurityAlertConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Bindings;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models.Graph;
    using Services;
    using Services.Graph;
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
            GraphService graphService;
            List<Alert> alerts;

            try
            {
                graphService = new GraphService(new Uri(input.Resource),
                    new ServiceCredentials(
                        input.ApplicationId,
                        await vault.GetSecretAsync(options.KeyVaultEndpoint, input.SecretName).ConfigureAwait(false),
                        input.Resource,
                        input.CustomerId));

                alerts = await graphService.GetAlertsAsync(cancellationToken).ConfigureAwait(false);

                return alerts;
            }
            catch (ServiceClientException ex)
            {
                log.LogError(ex, $"Encountered an error when processing {input.CustomerId}");
                return null;
            }
            finally
            {
                graphService = null;
            }
        }
    }
}