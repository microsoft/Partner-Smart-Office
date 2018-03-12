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

    public class GraphService : IGraphService
    {
        /// <summary>
        /// Provides the ability to perform HTTP operations.
        /// </summary>
        private IHttpService httpService;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphService" /> class.
        /// </summary>
        /// <param name="httpService">Provides the ability to perform HTTP operations.</param>
        /// <param name="endpoint">The Microsoft Graph service endpoint.</param>
        public GraphService(IHttpService httpService, string endpoint)
        {
            this.endpoint = endpoint;
            this.httpService = httpService;
        }

        /// <summary>
        /// Gets the approprtiate instance of the HTTP service.
        /// </summary>
        private IHttpService Http => httpService ?? HttpService.Instance;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public async Task<List<SecureScore>> GetSecureScoreAsync(IRequestContext requestContext, int period)
        {
            return await Http.GetAsync<List<SecureScore>>(
                new Uri($"{endpoint}/beta/reports/getTenantSecureScores(period={period})/content"),
                requestContext).ConfigureAwait(false);
        }
    }
}