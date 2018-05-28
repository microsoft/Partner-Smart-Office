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
        private readonly PartnerServiceClient client;

        private readonly string customerId;

        private readonly string subscriptionId;

        public SubscriptionOperations(PartnerServiceClient client, string customerId, string subscriptionId)
        {
            this.client = client;
            this.customerId = customerId;
            this.subscriptionId = subscriptionId;
        }

        public async Task<Subscription> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await client.GetAsync<Subscription>(
                new Uri($"/v1/customers/{customerId}/subscriptions/{subscriptionId}", UriKind.Relative),
                cancellationToken).ConfigureAwait(false);
        }
    }
}
