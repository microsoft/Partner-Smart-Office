// -----------------------------------------------------------------------
// <copyright file="Customer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter
{
    using System;
    using Newtonsoft.Json;

    public sealed class Customer
    {
        /// <summary>
        /// Gets or sets the company profile.
        /// </summary>
        [JsonProperty(PropertyName = "companyProfile")]
        public CompanyProfile CompanyProfile { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}