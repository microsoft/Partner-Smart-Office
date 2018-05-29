// -----------------------------------------------------------------------
// <copyright file="IOfferOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Offers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter.Offers;

    public interface IOfferOperations
    {
        Task<Offer> GetAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}