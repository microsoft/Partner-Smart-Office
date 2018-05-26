// -----------------------------------------------------------------------
// <copyright file="CustomerDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System;
    using Models.PartnerCenter;
    using Newtonsoft.Json;

    public sealed class CustomerDetail
    {
        /// <summary>
        /// Gets or sets the customer's company profile.
        /// </summary>
        [JsonProperty(PropertyName = "companyProfile")]
        public CompanyProfile CompanyProfile { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the customer.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the date the customer was last processed.
        /// </summary>
        [JsonProperty("lastProcessed")]
        public DateTimeOffset? LastProcessed { get; set; }
    }
}
