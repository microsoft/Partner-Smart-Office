// -----------------------------------------------------------------------
// <copyright file="GraphService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.SyncApp
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using IdentityModel.Clients.ActiveDirectory;
    using Models;
    using Newtonsoft.Json;
    using Rest;

    public class GraphService
    {
        private const string AuthenticationSchemeType = "Bearer";

        private static HttpClient client = new HttpClient(new RetryDelegatingHandler
        {
            InnerHandler = new HttpClientHandler()
        });

        public async Task<List<SecureScore>> GetSecureScoreAsync(string customerId, int period)
        {
            AuthenticationHeaderValue authHeader;
            HttpResponseMessage response;
            List<SecureScore> value = null;
            string content;

            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(
                    HttpMethod.Get,
                    new Uri(
                        $"{ConfigurationManager.AppSettings["GraphEndpoint"]}/beta/reports/getTenantSecureScores(period={period})/content")))
                {
                    authHeader = await GetAuthHeaderAsync(customerId).ConfigureAwait(false);
                    request.Headers.Authorization = authHeader;

                    response = await client.SendAsync(request).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        value = JsonConvert.DeserializeObject<List<SecureScore>>(content);
                    }
                }

                return value;
            }
            finally
            {
                response = null;
            }
        }

        private async Task<AuthenticationHeaderValue> GetAuthHeaderAsync(string customerId)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;

            try
            {
                authContext = new AuthenticationContext(
                    $"{ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"]}/{customerId}");

                authResult = await authContext.AcquireTokenAsync(
                    ConfigurationManager.AppSettings["GraphEndpoint"],
                    new ClientCredential(
                        ConfigurationManager.AppSettings["ApplicationId"],
                        ConfigurationManager.AppSettings["ApplicationSecret"])).ConfigureAwait(false);

                return new AuthenticationHeaderValue(AuthenticationSchemeType, authResult.AccessToken);
            }
            finally
            {
                authContext = null;
            }
        }
    }
}