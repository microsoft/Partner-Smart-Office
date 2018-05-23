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

    public interface ISubscriptionCollectionOperations
    {
        Task<SeekBasedResourceCollection<Subscription>> GetAsync(Link nextLink = null, CancellationToken cancellationToken = default(CancellationToken)); 
    }
}