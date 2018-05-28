// -----------------------------------------------------------------------
// <copyright file="OfferOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Offers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter.Offers;

    public class OfferOperations : IOfferOperations
    {
        private readonly PartnerServiceClient client;

        private readonly string country;

        private readonly string offerId;

        public OfferOperations(PartnerServiceClient client, string offerId, string country)
        {
            this.client = client;
            this.country = country;
            this.offerId = offerId;
        }

        public async Task<Offer> GetAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await client.GetAsync<Offer>(
               new Uri($"/v1/offers/{offerId}?country={country}", UriKind.Relative),
               cancellationToken).ConfigureAwait(false);
        }
    }
}