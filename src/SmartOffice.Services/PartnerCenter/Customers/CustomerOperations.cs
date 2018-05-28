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

        private readonly PartnerServiceClient client;

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

        public async Task<Customer> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await client.GetAsync<Customer>(
                new Uri($"/v1/customers/{customerId}", UriKind.Relative),
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the available subscription operations.
        /// </summary>
        public ISubscriptionCollectionOperations Subscriptions => subscriptions.Value;
    }
}