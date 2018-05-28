// -----------------------------------------------------------------------
// <copyright file="Order.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Orders
{
    using System;
    using System.Collections.Generic;
    using Offers;

    public sealed class Order : ResourceBaseWithLinks<StandardResourceLinks>
    {
        /// <summary>
        /// Gets or sets the type of billing cycle for the selected offers.
        /// </summary>
        public BillingCycleType BillingCycle { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the order.
        /// </summary>
        public DateTime? CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the indentifier of the order.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets line items for the order.
        /// </summary>
        public IEnumerable<OrderLineItem> LineItems { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the reference customer.
        /// </summary>
        public string ReferenceCustomerId { get; set; }
    }
}