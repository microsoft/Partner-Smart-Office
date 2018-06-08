// -----------------------------------------------------------------------
// <copyright file="PartnerServiceMessageHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    public class PartnerServiceMessageHandler : DelegatingHandler
    {
        /// <summary>
        /// Value for the accept header.
        /// </summary>
        private const string AcceptType = "application/json";

        /// <summary>
        /// Name of the application to be sent with each request.
        /// </summary>
        private const string ApplicationName = "Partner Smart Office v1.0.0";

        /// <summary>
        /// Name of the application name header. 
        /// </summary>
        private const string ApplicationNameHeader = "MS-PartnerCenter-Application";

        /// <summary>
        /// Name of the correlation identifier header.
        /// </summary>
        private const string CorrelationIdHeader = "MS-CorrelationId";

        /// <summary>
        /// Name of the request identifier header.
        /// </summary>
        private const string RequestIdHeader = "MS-RequestId";

        /// <summary>
        /// Sends a HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The response from the HTTP operation.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(AcceptType));
            request.Headers.Add(ApplicationNameHeader, ApplicationName);
            request.Headers.Add(CorrelationIdHeader, Guid.NewGuid().ToString());
            request.Headers.Add(RequestIdHeader, Guid.NewGuid().ToString());

            return base.SendAsync(request, cancellationToken);
        }
    }
}