// -----------------------------------------------------------------------
// <copyright file="CustomerCompanyProfile.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Customers
{
    using Newtonsoft.Json;

    public class CustomerCompanyProfile
    {
        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [JsonProperty("companyName")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or set the domain.
        /// </summary>
        [JsonProperty("domain")]
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the tenant.
        /// </summary>
        [JsonProperty("tenantId")]
        public string TenantId { get; set; }
    }
}