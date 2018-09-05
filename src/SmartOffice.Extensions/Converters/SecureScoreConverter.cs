// -----------------------------------------------------------------------
// <copyright file="DataRepositoryConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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

    public class SecureScoreConverter : IAsyncConverter<SecureScoreAttribute, List<SecureScore>>
    {
        private readonly ILogger log;

        private readonly IVaultService vault;

        private readonly SmartOfficeOptions options;

        public SecureScoreConverter(ILoggerFactory loggerFactory, IOptions<SmartOfficeOptions> options, IVaultService vault)
        {
            log = loggerFactory?.CreateLogger("Host.Bindings.SecureScoreConverter");
            this.options = options.Value;
            this.vault = vault;
        }

        public async Task<List<SecureScore>> ConvertAsync(SecureScoreAttribute input, CancellationToken cancellationToken)
        {
            GraphService graphService;
            List<SecureScore> secureScore;

            try
            {
                graphService = new GraphService(
                    new Uri(input.Resource),
                    new ServiceCredentials(
                        input.ApplicationId,
                        await vault.GetSecretAsync(options.KeyVaultEndpoint, input.SecretName).ConfigureAwait(false),
                        input.Resource,
                        input.CustomerId));

                secureScore = await graphService.GetSecureScoreAsync(int.Parse(input.Period, CultureInfo.CurrentCulture), cancellationToken).ConfigureAwait(false);

                return secureScore;
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