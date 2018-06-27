// -----------------------------------------------------------------------
// <copyright file="GraphService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;
    using Models.Graph;
    using Newtonsoft.Json;
    using Rest;
    using Rest.Serialization;

    public class GraphService : ServiceClient<GraphService>, IGraphService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphService" /> class.
        /// </summary>
        /// <param name="credentials">Credentials used when accessing resources.</param>
        /// <param name="handlers">List of handlers from top to bottom (outer handler is the first in the list)</param>
        public GraphService(ServiceClientCredentials credentials, params DelegatingHandler[] handlers)
            : base(handlers)
        {
            Credentials = credentials;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphService" /> class.
        /// </summary>
        /// <param name="endpoint">Address of the resource being accessed.</param>
        /// <param name="credentials">Credentials used when accessing resources.</param>
        /// <param name="handlers">List of handlers from top to bottom (outer handler is the first in the list)</param>
        public GraphService(Uri endpoint, ServiceClientCredentials credentials, params DelegatingHandler[] handlers)
            : base(handlers)
        {
            Credentials = credentials;
            Endpoint = endpoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphService" /> class.
        /// </summary>
        /// <param name="endpoint">Address of the resource being accessed.</param>
        /// <param name="credentials">Credentials used when accessing resources.</param>
        /// <param name="httpClient">The HTTP client to be used.</param>
        public GraphService(Uri endpoint, ServiceClientCredentials credentials, HttpClient httpClient)
            : base(httpClient, false)
        {
            Credentials = credentials;
            Endpoint = endpoint;
        }

        /// <summary>
        /// Gets or sets the credentials used when accessing resources.
        /// </summary>
        public ServiceClientCredentials Credentials { get; private set; }

        /// <summary>
        /// Gets or sets the address of the resource being accessed.
        /// </summary>
        public Uri Endpoint { get; private set; }

        /// <summary>
        /// Gets the alerts for the customer. 
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to monitor.</param>
        /// <returns>A list of alerts for the customer.</returns>
        public async Task<List<Alert>> GetAlertsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response = null;
            ODataResponse<Alert> alerts;
            string content;

            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(Endpoint, "/beta/security/alerts")))
                {
                    await Credentials.ProcessHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);

                    response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ServiceClientException(content, ((ServiceCredentials)Credentials).TenantId, response.StatusCode);
                    }

                    alerts = JsonConvert.DeserializeObject<ODataResponse<Alert>>(content);

                    return alerts.Value;
                }
            }
            finally
            {
                response = null;
            }
        }

        /// <summary>
        /// Gets the secure score for the defined period.
        /// </summary>
        /// <param name="period">Number of days of score results to retrieve starting from current date.</param>
        /// <param name="cancellationToken">The cancellation token to monitor.</param>
        /// <returns>A list of secure scores for the defined period.</returns>
        public async Task<List<SecureScore>> GetSecureScoreAsync(int period, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response = null;
            List<SecureScore> scores;
            string content;

            try
            {
                if (period <= 0 || period > 90)
                {
                    throw new InvalidOperationException($"{period} is an invalid period.");
                }

                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(Endpoint, $"/beta/reports/getTenantSecureScores(period={period})/content")))
                {
                    await Credentials.ProcessHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);

                    response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ServiceClientException(content, ((ServiceCredentials)Credentials).TenantId, response.StatusCode);
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