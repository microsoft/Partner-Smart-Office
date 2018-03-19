// -----------------------------------------------------------------------
// <copyright file="StorageServiceConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings.Converters
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Services;

    internal class StorageServiceConverter : IAsyncConverter<StorageServiceAttribute, StorageService>
    {
        private SmartOfficeExtensionConfig config;

        public StorageServiceConverter(SmartOfficeExtensionConfig config)
        {
            this.config = config;
        }

        public async Task<StorageService> ConvertAsync(StorageServiceAttribute input, CancellationToken cancellationToken)
        {
            await StorageService.Instance.InitializeAsync(
              input.KeyVaultEndpoint,
              input.ConnectionStringName).ConfigureAwait(false);

            return StorageService.Instance;
        }
    }
}
