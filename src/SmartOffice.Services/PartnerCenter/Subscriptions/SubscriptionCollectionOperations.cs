// -----------------------------------------------------------------------
// <copyright file="SubscriptionsCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Subscriptions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter;
    using Models.PartnerCenter.Subscriptions;
    
    public class SubscriptionCollectionOperations : ISubscriptionCollectionOperations
    {
        /// <summary>
        /// Provides the ability to perform HTTP operations.
        /// </summary>
        private readonly PartnerServiceClient client;

        /// <summary>
        /// Identifier for the customer.
        /// </summary>
        private readonly string customerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionCollectionOperations" /> class.
        /// </summary>
        /// <param name="client">Provides the ability to perform HTTP operations.</param>
        /// <param name="customerId">Identifier for the customer.</param>
        public SubscriptionCollectionOperations(PartnerServiceClient client, string customerId)
        {
            this.client = client;
            this.customerId = customerId;
        }

        public ISubscriptionOperations this[string subscriptionId] => ById(subscriptionId);

        public ISubscriptionOperations ById(string subscriptionId)
        {
            return new SubscriptionOperations(client, customerId, subscriptionId);
        }

        public async Task<SeekBasedResourceCollection<Subscription>> GetAsync(Link nextLink = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (nextLink == null)
            {
                return await client.GetAsync<SeekBasedResourceCollection<Subscription>>(
                    new Uri($"/v1/customers/{customerId}/subscriptions", UriKind.Relative), cancellationToken).ConfigureAwait(false);
            }

            return await client.GetAsync<SeekBasedResourceCollection<Subscription>>(
                nextLink, cancellationToken).ConfigureAwait(false);
        }
    }
}