// -----------------------------------------------------------------------
// <copyright file="CustomerCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Customers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter;

    public class CustomerCollectionOperations : ICustomerCollectionOperations
    {
        /// <summary>
        /// Provides the ability to perform HTTP operations.
        /// </summary>
        private readonly PartnerServiceClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerCollectionOperations" /> class.
        /// </summary>
        /// <param name="client">Provides the ability to perform HTTP operations.</param>
        public CustomerCollectionOperations(PartnerServiceClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Gets the customer operations for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of the <see cref="CustomerOperations" /> class.</returns>
        public ICustomerOperations this[string customerId] => ById(customerId);

        /// <summary>
        /// Gets the customer operations for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of the <see cref="CustomerOperations" /> class.</returns>
        public ICustomerOperations ById(string customerId)
        {
            return new CustomerOperations(client, customerId);
        }

        public async Task<SeekBasedResourceCollection<Customer>> GetAsync(Link nextLink = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (nextLink == null)
            {
                return await client.GetAsync<SeekBasedResourceCollection<Customer>>(
                    new Uri("/v1/customers", UriKind.Relative), cancellationToken).ConfigureAwait(false);
            }

            return await client.GetAsync<SeekBasedResourceCollection<Customer>>(
                nextLink, cancellationToken).ConfigureAwait(false);

        }
    }
}