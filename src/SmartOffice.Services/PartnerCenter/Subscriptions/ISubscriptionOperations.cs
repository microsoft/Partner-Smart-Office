// -----------------------------------------------------------------------
// <copyright file="ISubscriptionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Subscriptions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter.Subscriptions;
    using Utilization;

    public interface ISubscriptionOperations
    {
        IUtilizationCollectionOperations Utilization { get; }

        Task<Subscription> GetAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}