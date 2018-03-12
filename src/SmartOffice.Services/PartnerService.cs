// -----------------------------------------------------------------------
// <copyright file="PartnerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public class PartnerService : IPartnerService
    {
        private IHttpService httpService; 

        /// <summary>
        /// The Partner Center service endpoint.
        /// </summary>
        private string endpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerService" /> class.
        /// </summary>
        /// <param name="endpoint">The Partner Center service endpoint.</param>
        public PartnerService(string endpoint)
        {
            this.endpoint = endpoint;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerService" /> class.
        /// </summary>
        /// <param name="httpService">Provides the ability to perform HTTP operations.</param>
        /// <param name="endpoint">The Partner Center service endpoint.</param>
        public PartnerService(IHttpService httpService, string endpoint)
        {
            this.endpoint = endpoint;
            this.httpService = httpService;
        }

        /// <summary>
        /// Gets the approprtiate instance of the HTTP service.
        /// </summary>
        private IHttpService Http => httpService ?? HttpService.Instance;

        public async Task<List<Customer>> GetCustomersAsync(IRequestContext requestContext)
        {
            Resources<Customer> customers = await Http.GetAsync<Resources<Customer>>(
                new Uri($"{endpoint}/v1.0/customers"),
                requestContext).ConfigureAwait(false);

            return customers.Items;
        }
    }
}