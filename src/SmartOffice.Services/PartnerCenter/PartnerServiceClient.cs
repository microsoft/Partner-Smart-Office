// -----------------------------------------------------------------------
// <copyright file="PartnerServiceClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using AuditRecords;
    using Customers;
    using Models.PartnerCenter;
    using Models.PartnerCenter.JsonConverters;
    using Newtonsoft.Json;
    using Offers;
    using Rest;
    using Rest.Serialization;

    public class PartnerServiceClient : ServiceClient<PartnerServiceClient>, IPartnerServiceClient
    {
        /// <summary>
        /// The name of the client header.
        /// </summary>
        private const string ClientHeader = "MS-PartnerCenter-Client";

        /// <summary>
        /// The client connecting to the partner service.
        /// </summary>
        private const string PartnerCenterClient = "Partner Center Functions";

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerServiceClient" /> class.
        /// </summary>
        /// <param name="endpoint">Address of the resource being accessed.</param>
        /// <param name="credentials">Credentials used when accessing resources.</param>
        /// <param name="handlers">List of handlers from top to bottom (outer handler is the first in the list)</param>
        public PartnerServiceClient(Uri endpoint, ServiceClientCredentials credentials, params DelegatingHandler[] handlers)
            : base(handlers)
        {
            Credentials = credentials;
            Endpoint = endpoint;

            AuditRecords = new AuditRecordCollectionOperations(this);
            Customers = new CustomerCollectionOperations(this);
            Offers = new OfferCountrySelector(this);

            DeserializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                {
                    {
                        new EnumJsonConverter()
                    }
                },
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerServiceClient" /> class.
        /// </summary>
        /// <param name="endpoint">Address of the resource being accessed.</param>
        /// <param name="credentials">Credentials used when accessing resources.</param>
        /// <param name="httpClient">The HTTP client to be used.</param>
        public PartnerServiceClient(Uri endpoint, ServiceClientCredentials credentials, HttpClient httpClient)
            : base(httpClient, false)
        {
            Credentials = credentials;
            Endpoint = endpoint;

            AuditRecords = new AuditRecordCollectionOperations(this);
            Customers = new CustomerCollectionOperations(this);
            Offers = new OfferCountrySelector(this);

            DeserializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new ReadOnlyJsonContractResolver(),
                Converters = new List<JsonConverter>
                {
                    {
                        new EnumJsonConverter()
                    }
                },
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
        }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        public JsonSerializerSettings DeserializationSettings { get; private set; }

        /// <summary>
        /// Gets the credentials used when accessing resources.
        /// </summary>
        public ServiceClientCredentials Credentials { get; private set; }

        /// <summary>
        /// Gets or sets the address of the resource being accessed.
        /// </summary>
        public Uri Endpoint { get; private set; }

        /// <summary>
        /// Gets the available audit record operations.
        /// </summary>
        public IAuditRecordCollectionOperations AuditRecords { get; private set; }

        /// <summary>
        /// Gets the available customer operations.
        /// </summary>
        public ICustomerCollectionOperations Customers { get; private set; }

        /// <summary>
        /// Gets the available offer operations.
        /// </summary>
        public ICountrySelector<IOfferCollectionOperations> Offers { get; private set; }

        internal async Task<TResource> GetAsync<TResource>(Uri relativeUri, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response = null;
            string content;

            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(Endpoint, relativeUri)))
                {
                    await Credentials.ProcessHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);

                    request.Headers.Add(ClientHeader, PartnerCenterClient);

                    response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ServiceClientException(content, response.StatusCode);
                    }

                    return SafeJsonConvert.DeserializeObject<TResource>(content, DeserializationSettings);
                }
            }
            finally
            {
                response?.Dispose();
            }
        }


        internal async Task<TResource> GetAsync<TResource>(Link link, CancellationToken cancellationToken = default(CancellationToken))
        {
            HttpResponseMessage response = null;
            string content;

            try
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(Endpoint, $"/v1/{link.Uri}")))
                {
                    await Credentials.ProcessHttpRequestAsync(request, cancellationToken).ConfigureAwait(false);

                    request.Headers.Add(ClientHeader, PartnerCenterClient);

                    foreach (KeyValuePair<string, string> header in link.Headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }

                    response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                    content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ServiceClientException(content, response.StatusCode);
                    }

                    return SafeJsonConvert.DeserializeObject<TResource>(content, DeserializationSettings);
                }
            }
            finally
            {
                response?.Dispose();
            }
        }
    }
}