// -----------------------------------------------------------------------
// <copyright file="TokenConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings.Converters
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;

    internal class TokenConverter : IAsyncConverter<TokenAttribute, string>
    {
        /// <summary>
        /// Provides access to configuration information for the extension.
        /// </summary>
        private SmartOfficeExtensionConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenConverter" /> class.
        /// </summary>
        /// <param name="config">Provides access to configuration information for the extension.</param>
        public TokenConverter(SmartOfficeExtensionConfig config)
        {
            this.config = config;
        }

        public async Task<string> ConvertAsync(TokenAttribute input, CancellationToken cancellationToken)
        {

            return await config.GetAccessTokenAsync(
                $"https://login.microsoftonline.com/{input.ApplicationTenantId}",
                input.ApplicationId,
                input.SecretName,
                input.Resource).ConfigureAwait(false);
        }
    }
}