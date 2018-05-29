// -----------------------------------------------------------------------
// <copyright file="ISubscriptionsCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Subscriptions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter;
    using Models.PartnerCenter.Subscriptions;

    public interface ISubscriptionCollectionOperations
    {
        ISubscriptionOperations this[string subscriptionId] { get; }

        ISubscriptionOperations ById(string subscriptionId);

        Task<SeekBasedResourceCollection<Subscription>> GetAsync(Link nextLink = null, CancellationToken cancellationToken = default(CancellationToken)); 
    }
}