// -----------------------------------------------------------------------
// <copyright file="Subscription.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter
{
    using System;
    using Newtonsoft.Json;

    public sealed class Subscription : ResourceBaseWithLinks<StandardResourceLinks>
    {
        /// <summary>
        /// Gets or sets the date when the subscription was created.
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the friendly name for the subscription.
        /// </summary>
        [JsonProperty(PropertyName = "friendlyName")]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the offer identifier.
        /// </summary>
        [JsonProperty(PropertyName = "offerId")]
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets the quantity. 
        /// </summary>
        /// <remarks>
        /// If the subscription is licensed basd this value is considered the seat count.
        /// </remarks>
        [JsonProperty(PropertyName = "quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the subscription status.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public SubscriptionStatus Status { get; set; }
    }
}