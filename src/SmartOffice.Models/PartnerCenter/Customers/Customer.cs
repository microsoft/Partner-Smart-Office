// -----------------------------------------------------------------------
// <copyright file="Customer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Customers
{
    using Newtonsoft.Json;

    public sealed class Customer : ResourceBaseWithLinks<StandardResourceLinks>
    {
        [JsonProperty("billingProfile")]
        public CustomerBillingProfile BillingProfile { get; set; }

        /// <summary>
        /// Gets or sets the customer's company profile.
        /// </summary>
        [JsonProperty("companyProfile")]
        public CustomerCompanyProfile CompanyProfile { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the customer.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}