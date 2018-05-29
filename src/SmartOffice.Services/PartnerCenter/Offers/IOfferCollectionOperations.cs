// -----------------------------------------------------------------------
// <copyright file="IOfferCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Offers
{
    public interface IOfferCollectionOperations
    {
        IOfferOperations this[string offerId] { get; }

        IOfferOperations ById(string offerId);
    }
}