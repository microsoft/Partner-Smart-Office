// -----------------------------------------------------------------------
// <copyright file="DataRepositoryConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions.Converters
{
    using System;
    using System.Collections.Concurrent;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Bindings;
    using Microsoft.Extensions.Options;
    using Services;
    using Services.KeyVault;
    using Services.PartnerCenter;

    public class PartnerServiceConverter : IAsyncConverter<PartnerServiceAttribute, PartnerServiceClient>
    {
        /// <summary>
        /// Collection of HttpClient object used to communicate with the Partner Center API.
        /// </summary>
        private static readonly ConcurrentDictionary<string, HttpClient> partnerHttpClients = new ConcurrentDictionary<string, HttpClient>();

        /// <summary>
        /// Used to help ensure that the partner HTTP clients collection is accessed in a thread safe manner.
        /// </summary>
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private readonly IVaultService vault;

        private readonly SmartOfficeOptions options;

        public PartnerServiceConverter(IOptions<SmartOfficeOptions> options, IVaultService vault)
        {
            this.options = options.Value;
            this.vault = vault;
        }

        public async Task<PartnerServiceClient> ConvertAsync(PartnerServiceAttribute input, CancellationToken cancellationToken)
        {
            return new PartnerServiceClient(
                new Uri(input.Endpoint),
                new ServiceCredentials(
                    input.ApplicationId,
                    await vault.GetSecretAsync(options.KeyVaultEndpoint, input.SecretName).ConfigureAwait(false),
                    input.Resource,
                    input.ApplicationTenantId),
                await GetPartnerHttpClientAsync(input.ApplicationTenantId).ConfigureAwait(false));
        }

        private static async Task<HttpClient> GetPartnerHttpClientAsync(string key)
        {
            try
            {
                await Semaphore.WaitAsync().ConfigureAwait(false);

                if (!partnerHttpClients.ContainsKey(key))
                {
                    partnerHttpClients[key] = HttpClientFactory.Create(new PartnerServiceMessageHandler()); ;
                }

                return partnerHttpClients[key];
            }
            finally
            {
                Semaphore.Release();
            }
        }

    }
}