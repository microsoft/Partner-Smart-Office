// -----------------------------------------------------------------------
// <copyright file="GraphService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;
    using Rest;
    using Rest.Serialization;

    public class GraphService : ServiceClient<GraphService>, IGraphService
    {
        public GraphService(ServiceClientCredentials credentials, params DelegatingHandler[] handlers) : base(handlers)
        {
            Credentials = credentials;
        }

        public GraphService(Uri endpoint, ServiceClientCredentials credentials, params DelegatingHandler[] handlers) : base(handlers)
        {
            Credentials = credentials;
            Endpoint = endpoint;
        }

        public ServiceClientCredentials Credentials { get; private set; }

        public Uri Endpoint { get; private set; }

        public async Task<List<SecureScore>> GetSecureScoreAsync(int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response = null;
            List<SecureScore> scores;
            string content;

            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{Endpoint}/beta/reports/getTenantSecureScores(period={period})/content")))
                {
                    await Credentials.ProcessHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);

                    response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(content);
                    }

                    scores = SafeJsonConvert.DeserializeObject<List<SecureScore>>(content);

                    return scores;
                }
            }
            finally
            {
                response?.Dispose();
            }
        }
    }
}