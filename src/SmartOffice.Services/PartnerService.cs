// -----------------------------------------------------------------------
// <copyright file="PartnerService.cs" company="Microsoft">
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

    public class PartnerService : ServiceClient<PartnerService>, IPartnerService
    {
        public PartnerService(ServiceClientCredentials credentials, params DelegatingHandler[] handlers) : base(handlers)
        {
            Credentials = credentials;
        }

        public PartnerService(Uri endpoint, ServiceClientCredentials credentials, params DelegatingHandler[] handlers) : base(handlers)
        {
            Credentials = credentials;
            Endpoint = endpoint;
        }

        public ServiceClientCredentials Credentials { get; private set; }

        public Uri Endpoint { get; private set; }

        /// <summary>
        /// Gets a list of available customers.
        /// </summary>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests.
        /// </param>
        /// <returns>A list of available customers.</returns>
        public async Task<List<Customer>> GetCustomersAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response = null;
            Resources<Customer> customers;
            string content;

            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri($"{Endpoint}/v1.0/customers")))
                {
                    await Credentials.ProcessHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);

                    response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(content);
                    }

                    customers = SafeJsonConvert.DeserializeObject<Resources<Customer>>(content);

                    return customers.Items;
                }
            }
            finally
            {
                response?.Dispose();
            }
        }
    }
}