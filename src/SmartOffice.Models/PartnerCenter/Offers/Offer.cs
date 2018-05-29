// -----------------------------------------------------------------------
// <copyright file="Offer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Offers
{
    using System;
    using System.Collections.Generic;
    using Subscriptions;

    public sealed class Offer
    {
        /// <summary>
        /// Gets or sets how billing is handled for the item purchase.
        /// </summary>
        public BillingType Billing { get; set; }

        /// <summary>
        /// Gets or sets the offer category.
        /// </summary>
        public OfferCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the country where the offer applies.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the description of the offer.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the idntifier of the offer.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether or not the offer is an add-on.
        /// </summary>
        public bool IsAddOn { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether or not the offer is auto renewable.
        /// </summary>
        public bool IsAutoRenewable { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether or not the offer is available for purchase.
        /// </summary>
        public bool IsAvailableForPurchase { get; set; }

        /// <summary>
        /// Gets or sets the locale where the offer applies.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the minimum quantity available.
        /// </summary>
        public int MinimumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the maximum quantity available.
        /// </summary>
        public int MaximumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the name of the offer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets prerequisites for the offer.
        /// </summary>
        public IEnumerable<string> PrerequisiteOffers { get; set; }

        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Gets or sets the category rank in the offer collection.
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the sales group.
        /// </summary>
        public string SalesGroupId { get; set; }

        /// <summary>
        /// Gets or sets the supported billing cycles for the offer.
        /// </summary>
        public IEnumerable<BillingCycleType> SupportedBillingCycles { get; set; }

        /// <summary>
        /// Gets or sets the list of offers that this offer can be upgraded to. 
        /// </summary>
        public IEnumerable<string> UpgradeTargetOffers { get; set; }

        /// <summary>
        /// Gets or sets the unit type.
        /// </summary>
        public string UnitType { get; set; }

        /// <summary>
        /// Gets or sets the offer uniform resource identifier (URI).
        /// </summary>
        public Uri Uri { get; set; }
    }
}