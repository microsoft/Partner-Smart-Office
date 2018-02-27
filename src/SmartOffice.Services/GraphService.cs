// -----------------------------------------------------------------------
// <copyright file="GraphService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public class GraphService
    {
        /// <summary>
        /// The Microsoft Graph service endpoint.
        /// </summary>
        private string endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphService" /> class.
        /// </summary>
        /// <param name="endpoint">The Microsoft Graph service endpoint.</param>
        public GraphService(string endpoint)
        {
            this.endpoint = endpoint;
        }

        public async Task<List<SecureScore>> GetSecureScoreAsync(IRequestContext requestContext, int period)
        {
            return await HttpService.Instance.GetAsync<List<SecureScore>>(
                new Uri($"{endpoint}/beta/reports/getTenantSecureScores(period={period})/content"),
                requestContext).ConfigureAwait(false);
        }
    }
}