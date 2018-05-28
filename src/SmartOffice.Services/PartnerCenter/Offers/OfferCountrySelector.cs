// -----------------------------------------------------------------------
// <copyright file="OfferCountrySelector.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Offers
{
    internal class OfferCountrySelector : ICountrySelector<IOfferCollectionOperations>
    {
        private readonly PartnerServiceClient client;

        public OfferCountrySelector(PartnerServiceClient client)
        {
            this.client = client;
        }

        public IOfferCollectionOperations ByCountry(string country)
        {
            return new OfferCollectionOperations(client, country);
        }
    }
}