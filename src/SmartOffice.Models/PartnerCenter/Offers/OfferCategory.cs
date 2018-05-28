// -----------------------------------------------------------------------
// <copyright file="OfferCategory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Offers
{
    public sealed class OfferCategory
    {
        public string Country { get; set; }

        public string Id { get; set; }

        public string Locale { get; set; }

        public string Name { get; set; }

        public int Rank { get; set; }
    }
}