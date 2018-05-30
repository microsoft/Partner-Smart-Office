// -----------------------------------------------------------------------
// <copyright file="SubscriptionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Subscriptions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter.Subscriptions;

    public class SubscriptionOperations : ISubscriptionOperations
    {
        /// <summary>
        /// Provides the ability to interact with Partner Center.
        /// </summary>
        private readonly PartnerServiceClient client;

        /// <summary>
        /// The identifier for the customer. 
        /// </summary>
        private readonly string customerId;

        /// <summary>
        /// THe identifier for the subscription.
        /// </summary>
        private readonly string subscriptionId;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionOperations" /> class.
        /// </summary>
        /// <param name="client">Provides the ability to interact with Partner Center.</param>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <param name="subscriptionId">Identifier for the subscription.</param>
        public SubscriptionOperations(PartnerServiceClient client, string customerId, string subscriptionId)
        {
            this.client = client;
            this.customerId = customerId;
            this.subscriptionId = subscriptionId;
        }

        /// <summary>
        /// Gets the subscription innformation.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Information for the specified subscription.</returns>
        public async Task<Subscription> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await client.GetAsync<Subscription>(
                new Uri($"/v1/customers/{customerId}/subscriptions/{subscriptionId}", UriKind.Relative),
                cancellationToken).ConfigureAwait(false);
        }
    }
}
