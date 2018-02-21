// -----------------------------------------------------------------------
// <copyright file="Customer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public sealed class Customer
    {
        /// <summary>
        /// Gets or set the company name for the customer.
        /// </summary>
        [JsonProperty(PropertyName = "companyName")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the list of subscriptions for the customer.
        /// </summary>
        [JsonProperty(PropertyName = "subscriptions")]
        public List<Subscription> Subscriptions { get; set; }
    }
}