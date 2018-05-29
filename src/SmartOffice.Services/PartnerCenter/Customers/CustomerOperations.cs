// -----------------------------------------------------------------------
// <copyright file="CustomerOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Customers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter.Customers;
    using Subscriptions;

    public class CustomerOperations : ICustomerOperations
    {
        /// <summary>
        /// Provides the available subscription operations.
        /// </summary>
        private readonly Lazy<ISubscriptionCollectionOperations> subscriptions;

        /// <summary>
        /// Provides the ability to perform HTTP operations.
        /// </summary>
        private readonly PartnerServiceClient client;

        /// <summary>
        /// The identifier for the customer.
        /// </summary>
        private readonly string customerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerOperations" /> class.
        /// </summary>
        /// <param name="client">Provides the ability to perform HTTP operations.</param>
        /// <param name="customerId">Identifier for the customer.</param>
        public CustomerOperations(PartnerServiceClient client, string customerId)
        {
            this.client = client;
            this.customerId = customerId;
            subscriptions = new Lazy<ISubscriptionCollectionOperations>(() => new SubscriptionCollectionOperations(client, customerId));
        }

        /// <summary>
        /// Gets the information for the customer.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The information for the customer.</returns>
        public async Task<Customer> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await client.GetAsync<Customer>(
                new Uri($"/v1/customers/{customerId}", UriKind.Relative),
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Obtains the subscriptions behavior for the customer.
        /// </summary>
        public ISubscriptionCollectionOperations Subscriptions => subscriptions.Value;
    }
}