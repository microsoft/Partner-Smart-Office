// -----------------------------------------------------------------------
// <copyright file="OrderLineItem.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Orders
{
    public sealed class OrderLineItem 
    {
        public string FriendlyName { get; set; }

        public int LineItemNumber { get; set; }

        public string OfferId { get; set; }

        public string PartnerIdOnRecord { get; set; }

        public string ParentSubscriptionId { get; set; }

        public int Quantity { get; set; }

        public string SubscriptionId { get; set; }
    }
}