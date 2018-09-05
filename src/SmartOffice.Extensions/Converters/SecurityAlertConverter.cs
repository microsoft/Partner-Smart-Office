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
    using Microsoft.Extensions.Options;
    using Models.Graph;
    using Services;
    using Services.Graph;
    using Services.KeyVault;

    public class SecurityAlertConverter : IAsyncConverter<SecurityAlertsAttribute, List<Alert>>
    {
        private readonly IVaultService vault;

        private readonly SmartOfficeOptions options;

        public SecurityAlertConverter(IOptions<SmartOfficeOptions> options, IVaultService vault)
        {
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
            catch (ServiceClientException)
            {
                // log.LogError(ex, $"Encountered {ex.Message} when processing {input.CustomerId}");
                return null;
            }
            finally
            {
                graphService = null;
            }
        }
    }
}