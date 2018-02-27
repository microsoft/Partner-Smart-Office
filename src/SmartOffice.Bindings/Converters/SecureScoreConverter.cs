// -----------------------------------------------------------------------
// <copyright file="SecureScoreConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings.Converters
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Models;
    using Services;

    public class SecureScoreConverter : IAsyncConverter<SecureScoreAttribute, SecureScore>
    {
        /// <summary>
        /// Provides access to application settings.
        /// </summary>
        private INameResolver appSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecureScoreConverter" /> class.
        /// </summary>
        /// <param name="appSettings">Provides access to application settings.</param>
        public SecureScoreConverter(INameResolver appSettings)
        {
            this.appSettings = appSettings;
        }

        public async Task<SecureScore> ConvertAsync(SecureScoreAttribute input, CancellationToken cancellationToken)
        {
            GraphService graphService;
            List<SecureScore> secureScore;

            try
            {
                graphService = new GraphService(input.Resource);

                secureScore = await graphService.GetSecureScoreAsync(
                    new RequestContext
                    {
                    },
                    input.Period).ConfigureAwait(false);

                return secureScore[0];
            }
            finally
            {
                graphService = null;
            }
        }
    }
}