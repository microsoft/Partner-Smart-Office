// -----------------------------------------------------------------------
// <copyright file="HttpService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Rest;

    internal class HttpService : IHttpService
    {
        /// <summary>
        /// Static instance of the <see cref="HttpClient" /> class.
        /// </summary>
        private static HttpClient client = new HttpClient(
            new RetryDelegatingHandler
            {
                InnerHandler = new HttpClientHandler()
            });

        /// <summary>
        /// Singleton instance of the <see cref="HttpService" /> class.
        /// </summary>
        private static Lazy<HttpService> instance = new Lazy<HttpService>(() => new HttpService());

        /// <summary>
        /// Type of schemem to be used for authorization.
        /// </summary>
        private const string AuthenticationSchemeType = "Bearer";

        /// <summary>
        /// Name of the correlation identifier header.
        /// </summary>
        private const string CorrelationIdHeader = "MS-CorrelationId";

        /// <summary>
        /// MIME type for JSON media.
        /// </summary>
        private const string JsonMediaType = "application/json";

        /// <summary>
        /// Name of the locale header.
        /// </summary>
        private const string LocaleHeader = "X-Locale";

        /// <summary>
        /// Name of the request identifier header.
        /// </summary>
        private const string RequestIdHeader = "MS-RequestId";

        /// <summary>
        /// Gets an instance of the <see cref="HttpService" /> class.
        /// </summary>
        public static HttpService Instance => instance.Value;

        public async Task<TResponse> GetAsync<TResponse>(Uri requestUri, IRequestContext requestContext)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                InjectHeaders(requestContext, request);

                return await HandleResponseAsync<TResponse>(
                    await client.SendAsync(request).ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        private async Task<TResponse> HandleResponseAsync<TResponse>(HttpResponseMessage response)
        {
            string content = (response.Content == null) ?
                string.Empty : await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                // TODO - Throw a specific exception instead of a general one. 
                throw new Exception(content);
            }

            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        private void InjectHeaders(IRequestContext requestContext, HttpRequestMessage request)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));

            request.Headers.Add(CorrelationIdHeader, requestContext.CorrelationId.ToString());
            request.Headers.Add(LocaleHeader, requestContext.Locale);
            request.Headers.Add(RequestIdHeader, requestContext.RequestId.ToString());

            request.Headers.Authorization = new AuthenticationHeaderValue(
                AuthenticationSchemeType,
                requestContext.AccessToken);
        }
    }
}