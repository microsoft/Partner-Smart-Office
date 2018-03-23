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
        /// <summary>
        /// Provides access to configuration information for the extension.
        /// </summary>
        private SmartOfficeExtensionConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageServiceConverter" /> class.
        /// </summary>
        /// <param name="config">Provides access to configuration information for the extension.</param>
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