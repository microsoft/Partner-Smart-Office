// -----------------------------------------------------------------------
// <copyright file="OfferCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Offers
{
    public class OfferCollectionOperations : IOfferCollectionOperations
    {
        private readonly PartnerServiceClient client;

        private readonly string country;

        public OfferCollectionOperations(PartnerServiceClient client, string country)
        {
            this.client = client;
            this.country = country;
        }

        public IOfferOperations this[string offerId] => ById(offerId);

        public IOfferOperations ById(string offerId)
        {
            return new OfferOperations(client, offerId, country);
        }
    }
}