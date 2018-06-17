// -----------------------------------------------------------------------
// <copyright file="CustomerDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System;
    using Models.PartnerCenter.Customers;
    using Newtonsoft.Json;

    public sealed class CustomerDetail
    {
        /// <summary>
        /// Gets or sets the customer's billing profile.
        /// </summary>
        [JsonProperty("billingProfile")]
        public CustomerBillingProfile BillingProfile { get; set; }

        /// <summary>
        /// Gets or sets the customer's company profile.
        /// </summary>
        [JsonProperty(PropertyName = "companyProfile")]
        public CustomerCompanyProfile CompanyProfile { get; set; }

        /// <summary>
        /// Gets or sets the environment identifier.
        /// </summary>
        [JsonProperty("environmentId")]
        public string EnvironmentId { get; set; }

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

        /// <summary>
        /// Gets or sets the exception that was encountered when processing the customer.
        /// </summary>
        [JsonProperty("processException")]
        public Exception ProcessException { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether or not the customer was removed from Partner Center.
        /// </summary>
        [JsonProperty("removedFromPartnerCenter")]
        public bool RemovedFromPartnerCenter { get; set; }
    }
}