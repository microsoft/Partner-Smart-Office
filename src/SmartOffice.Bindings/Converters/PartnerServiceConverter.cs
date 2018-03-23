// -----------------------------------------------------------------------
// <copyright file="PartnerServiceConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings.Converters
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Services;

    internal class PartnerServiceConverter : IAsyncConverter<PartnerServiceAttribute, PartnerService>
    {
        /// <summary>
        /// Name of the Key Vault endpoint setting.
        /// </summary>
        private const string KeyVaultEndpoint = "KeyVaultEndpoint";

        /// <summary>
        /// Provides access to configuration information for the extension.
        /// </summary>
        private SmartOfficeExtensionConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartOfficeExtensionConfig" /> class.
        /// </summary>
        /// <param name="config">Provides access to configuration information for the extension.</param>
        public PartnerServiceConverter(SmartOfficeExtensionConfig config)
        {
            this.config = config;
        }

        public async Task<PartnerService> ConvertAsync(PartnerServiceAttribute input, CancellationToken cancellationToken)
        {
            IVaultService vaultService;

            try
            {
                vaultService = new KeyVaultService(config.AppSettings.Resolve(KeyVaultEndpoint));

                return new PartnerService(new Uri("https://api.partnercenter.microsoft.com"),
                    new ServiceCredentials(
                        input.ApplicationId,
                        await vaultService.GetSecretAsync(input.SecretName).ConfigureAwait(false),
                        input.Resource,
                        input.ApplicationTenantId));
            }
            finally
            {
                vaultService = null;
            }
        }
    }
}