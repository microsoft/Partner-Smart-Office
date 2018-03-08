// -----------------------------------------------------------------------
// <copyright file="SecureScoreConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Models;
    using Services;

    internal class SecureScoreConverter : IAsyncConverter<SecureScoreAttribute, List<SecureScore>>
    {
        /// <summary>
        /// Provides access to configuration information for the extension.
        /// </summary>
        private SmartOfficeExtensionConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenConverter" /> class.
        /// </summary>
        /// <param name="config">Provides access to configuration information for the extension.</param>
        public SecureScoreConverter(SmartOfficeExtensionConfig config)
        {
            this.config = config;
        }

        public async Task<List<SecureScore>> ConvertAsync(SecureScoreAttribute input, CancellationToken cancellationToken)
        {
            GraphService graphService;
            List<SecureScore> secureScore;

            try
            {
                graphService = new GraphService(input.Resource);

                secureScore = await graphService.GetSecureScoreAsync(
                    new RequestContext
                    {
                        AccessToken = await config.GetAccessTokenAsync(
                            $"https://login.microsoftonline.com/{input.CustomerId}",
                            input.ApplicationId,
                            input.SecretName,
                            input.Resource).ConfigureAwait(false),
                        CorrelationId = Guid.NewGuid()
                    },
                    input.Period).ConfigureAwait(false);

                return secureScore;
            }
            finally
            {
                graphService = null;
            }
        }
    }
}