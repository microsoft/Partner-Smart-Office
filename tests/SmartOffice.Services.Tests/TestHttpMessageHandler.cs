// -----------------------------------------------------------------------
// <copyright file="TestHttpMessageHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.KeyVault.Models;
    using Newtonsoft.Json;

    public class TestHttpMessageHandler : DelegatingHandler
    {
        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server 
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>The response from the server.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string content = string.Empty;

            await Task.FromResult(0).ConfigureAwait(false);

            if (request.RequestUri.LocalPath.Equals("/secrets/UnitTest/", StringComparison.InvariantCultureIgnoreCase))
            {
                content = JsonConvert.SerializeObject(new SecretBundle("AmazingSecret", "https://smartoffice.test/secrets/UnitTest", "text/plain"));
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content)
            };
        }
    }
}