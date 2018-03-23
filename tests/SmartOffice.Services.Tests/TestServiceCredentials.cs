// -----------------------------------------------------------------------
// <copyright file="PartnerServiceTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Tests
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks; 
    using Rest; 

    public class TestServiceCredentials : ServiceClientCredentials
    {
        private const string AuthenticationScheme = "Bearer";

        private string token; 

        public TestServiceCredentials(string token)
        {
            this.token = token; 
        }

        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(AuthenticationScheme, token);
            await base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}