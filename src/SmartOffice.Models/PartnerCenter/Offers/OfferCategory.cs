// -----------------------------------------------------------------------
// <copyright file="OfferCategory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Offers
{
    public sealed class OfferCategory
    {
        /// <summary>
        /// Gets or sets the country where the offer category applies.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the offer category.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the locale where the offer category applies.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the name of the offer category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category rank in the collection.
        /// </summary>
        public int Rank { get; set; }
    }
}