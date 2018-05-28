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
        public BillingCycleType BillingCycle { get; set; }

        public DateTime? CreationDate { get; set; }

        public string Id { get; set; }

        public IEnumerable<OrderLineItem> LineItems { get; set; }

        public string ReferenceCustomerId { get; set; }
    }
}