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
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerService" /> class.
        /// </summary>
        /// <param name="credentials">Credentials used when accessing resources.</param>
        /// <param name="handlers">List of handlers from top to bottom (outer handler is the first in the list)</param>
        public PartnerService(ServiceClientCredentials credentials, params DelegatingHandler[] handlers) : base(handlers)
        {
            Credentials = credentials;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerService" /> class.
        /// </summary>
        /// <param name="endpoint">Address of the resource being accessed.</param>
        /// <param name="credentials">Credentials used when accessing resources.</param>
        /// <param name="handlers">List of handlers from top to bottom (outer handler is the first in the list)</param>
        public PartnerService(Uri endpoint, ServiceClientCredentials credentials, params DelegatingHandler[] handlers) : base(handlers)
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