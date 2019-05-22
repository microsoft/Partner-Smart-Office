// -----------------------------------------------------------------------
// <copyright file="PartnerServiceConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions.Converters
{
    using System.Collections.Concurrent;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Microsoft.Extensions.Options;
    using Services.KeyVault;
    using Store.PartnerCenter;
    using Store.PartnerCenter.Extensions;

    public class PartnerServiceConverter : IAsyncConverter<PartnerServiceAttribute, IPartner>
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

        public async Task<IPartner> ConvertAsync(PartnerServiceAttribute input, CancellationToken cancellationToken)
        {
            return PartnerService.Instance.CreatePartnerOperations(
                await PartnerCredentials.GenerateByApplicationCredentialsAsync(
                    input.ApplicationId,
                    await vault.GetSecretAsync(options.KeyVaultEndpoint, input.SecretName).ConfigureAwait(false),
                    input.ApplicationTenantId).ConfigureAwait(false),
                await GetPartnerHttpClientAsync(input.ApplicationTenantId).ConfigureAwait(false));
        }

        private static async Task<HttpClient> GetPartnerHttpClientAsync(string key)
        {
            try
            {
                await Semaphore.WaitAsync().ConfigureAwait(false);

                if (!partnerHttpClients.ContainsKey(key))
                {
                    partnerHttpClients[key] = HttpClientFactory.Create();
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