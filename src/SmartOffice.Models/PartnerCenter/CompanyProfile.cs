// -----------------------------------------------------------------------
// <copyright file="CompanyProfile.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter
{
    using Newtonsoft.Json;

    public class CompanyProfile
    {
        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [JsonProperty(PropertyName = "companyName")]
        public string CompanyName { get; set; }

        /// <summary>
        /// Gets or set the domain.
        /// </summary>
        [JsonProperty(PropertyName = "domain")]
        public string Domain { get; set; }
    }
}