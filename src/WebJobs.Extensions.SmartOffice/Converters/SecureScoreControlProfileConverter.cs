// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.WebJobs.Extensions.SmartOffice
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Graph;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Provides the functionality to convert the input binding parameter to a collection of Secure Score control profiles. 
    /// </summary>
    internal class SecureScoreControlProfileConverter : IAsyncConverter<SecureScoreControlProfileAttribute, List<SecureScoreControlProfile>>
    {
        /// <summary>
        /// The configuration provider for the Smart Office extension.
        /// </summary>
        private readonly SmartOfficeExtensionConfigProvider provider;

        /// <summary>
        /// Initializes an instance of the <see cref="SecureScoreControlProfileConverter" /> class.
        /// </summary>
        /// <param name="provider">The configuration provider for the Smart Office extension.</param>
        public SecureScoreControlProfileConverter(SmartOfficeExtensionConfigProvider provider)
        {
            provider.AssertNotNull(nameof(provider));

            this.provider = provider;
        }

        /// <summary>
        /// Converts the input parameter to a collection of Secure Score control profiles.
        /// </summary>
        /// <param name="input">The input binding parameter used to perform the conversion.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of Secure Score control profiles.</returns>
        public async Task<List<SecureScoreControlProfile>> ConvertAsync(SecureScoreControlProfileAttribute input, CancellationToken cancellationToken)
        {
            GraphServiceClient serviceClient;
            ISecuritySecureScoreControlProfilesCollectionPage page;
            List<SecureScoreControlProfile> items = null;

            try
            {
                serviceClient = provider.GetServiceClient(input);
                page = await serviceClient.Security.SecureScoreControlProfiles.Request().GetAsync(cancellationToken).ConfigureAwait(false);
                items = new List<SecureScoreControlProfile>(page.CurrentPage);

                while (page.NextPageRequest != null)
                {
                    page = await page.NextPageRequest.GetAsync(cancellationToken).ConfigureAwait(false);
                    items.AddRange(page.CurrentPage);
                }
            }
            catch (ClientException ex)
            {
                provider.Logger.Log(LogLevel.Error, ex, $"Encountered exception when requesting information for {input.CustomerId}");
            }

            return items;
        }
    }
}
