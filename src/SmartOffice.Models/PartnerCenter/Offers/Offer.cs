// -----------------------------------------------------------------------
// <copyright file="Offer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Offers
{
    using System;
    using System.Collections.Generic;

    public sealed class Offer
    {
        public BillingType Billing { get; set; }

        public OfferCategory Category { get; set; }

        public string Country { get; set; }

        public string Description { get; set; }

        public string Id { get; set; }

        public bool IsAddOn { get; set; }

        public bool IsAutoRenewable { get; set; }

        public bool IsAvailableForPurchase { get; set; }

        public string Locale { get; set; }

        public int MinimumQuantity { get; set; }

        public int MaximumQuantity { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> PrerequisiteOffers { get; set; }

        public Product Product { get; set; }

        public int Rank { get; set; }

        public string SalesGroupId { get; set; }

        public IEnumerable<BillingCycleType> SupportedBillingCycles { get; set; }

        public IEnumerable<string> UpgradeTargetOffers { get; set; }

        public string UnitType { get; set; }


        public Uri Uri { get; set; }
    }
}