// -----------------------------------------------------------------------
// <copyright file="SubscriptionDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using PartnerCenter.Offers;
    using PartnerCenter.Subscriptions;

    public sealed class SubscriptionDetail
    {
        /// <summary>
        /// Gets or sets a value indicating whether automatic renew is enabled or not.
        /// </summary>
        [JsonProperty("autoRenewEnabled")]
        public bool AutoRenewEnabled { get; set; }

        /// <summary>
        /// Gets or sets the billing cycle for the subscription.
        /// </summary>
        [JsonProperty("billingCycle")]
        public BillingCycleType BillingCycle { get; set; }

        /// <summary>
        /// Gets or sets the billing type for the subscription.
        /// </summary>
        [JsonProperty("billingType")]
        public BillingType BillingType { get; set; }

        /// <summary>
        /// Gets or sets the commitment end date for this subscription.
        /// </summary>
        [JsonProperty("commitmentEndDate")]
        public DateTime CommitmentEndDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the subscription was created.
        /// </summary>
        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the effective start date for this subscription.
        /// </summary>
        [JsonProperty("effectiveStartDate")]
        public DateTime EffectiveStartDate { get; set; }

        /// <summary>
        /// Gets or sets the subscription's friendly name.
        /// </summary>
        [JsonProperty("friendlyName")]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the subscription.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the offer identifier used to purchase the subscription.
        /// </summary>
        [JsonProperty("offerId")]
        public string OfferId { get; set; }

        /// <summary>
        /// Gets or sets the offer name used to purchase the subscription.
        /// </summary>
        [JsonProperty("offerName")]
        public string OfferName { get; set; }

        /// <summary>
        /// Gets or sets the parent subscription identifier.
        /// </summary>
        [JsonProperty("parentSubscriptionId")]
        public string ParentSubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the MPN identifier. This only applies to indirect partner scenarios.
        /// </summary>
        [JsonProperty("partnerId")]
        public string PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the subscription.
        /// </summary>
        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the status of the subscription.
        /// </summary>
        [JsonProperty("status")]
        public SubscriptionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the suspension reason for the subscription.
        /// </summary>
        [JsonProperty("suspensionReasons")]
        public IEnumerable<string> SuspensionReasons { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the tenant that owns the subscription.
        /// </summary>
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the units definig the quantity for the subscription.
        /// </summary>
        [JsonProperty("unitType")]
        public string UnitType { get; set; }
    }
}