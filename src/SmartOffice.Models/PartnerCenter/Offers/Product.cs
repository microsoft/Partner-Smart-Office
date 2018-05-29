// -----------------------------------------------------------------------
// <copyright file="Product.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Offers
{
    public sealed class Product
    {
        /// <summary>
        /// Gets or sets the identifier of the product.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the product unti.
        /// </summary>
        public string Unit { get; set; }
    }
}