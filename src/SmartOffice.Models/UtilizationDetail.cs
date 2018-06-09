// -----------------------------------------------------------------------
// <copyright file="UtilizationDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using PartnerCenter.Utilizations;

    public sealed class UtilizationDetail
    {
        /// <summary>
        /// Gets or sets the key-value pairs of instance-level details.
        /// </summary>
        [JsonProperty("infoFields")]
        public IDictionary<string, string> InfoFields { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the utilization detail.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the instance details.
        /// </summary>
        [JsonProperty("instanceData")]
        public AzureInstanceData InstanceData { get; set; }

        /// <summary>
        /// Gets or sets the quantity consumed of the Azure resource.
        /// </summary>
        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// Gets or sets the Azure resource which was used.
        /// </summary>
        [JsonProperty("resource")]
        public AzureResource Resource { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the subscription that owns the usage.
        /// </summary>
        [JsonProperty("subscriptionId")]
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier that owns the subscription.
        /// </summary>
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the type of quantity (hours, bytes, etc...).
        /// </summary>
        [JsonProperty("unit")]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the end of the usage aggregation time range.
        /// The response is grouped by the time of consumption (when the resource was actually used VS. when was it reported to the billing system).
        /// </summary>
        [JsonProperty("usageEndTime")]
        public DateTimeOffset UsageEndTime { get; set; }

        /// Gets or sets the start of the usage aggregation time range.
        /// The response is grouped by the time of consumption (when the resource was actually used VS. when was it reported to the billing system).
        /// </summary>
        [JsonProperty("usageStartTime")]
        public DateTimeOffset UsageStartTime { get; set; }
    }
}