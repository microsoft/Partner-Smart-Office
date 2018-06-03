// -----------------------------------------------------------------------
// <copyright file="EnvironmentDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class EnvironmentDetail
    {
        /// <summary>
        /// Gets or sets the application endpoint details for the environment.
        /// </summary>
        [Required]
        public EndpointDetail AppEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the type for the environment.
        /// </summary>
        [Display(Name = "Environment Type")]
        [Required]
        public EnvironmentType EnvironmentType { get; set; }

        /// <summary>
        /// Gets or sets the friendly name for the environment.
        /// </summary>
        [Display(Name = "Friendly Name")]
        [Required]
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the environment.
        /// </summary>
        [JsonProperty("id")]
        [RegularExpression(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$")]
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the time the environment was last processed.
        /// </summary>
        [Display(Name = "Last Processed")]
        [Required]
        public DateTimeOffset LastProcessed { get; set; }

        /// <summary>
        /// Gets or sets the Microsoft Partner Center endpoint details for the environment.
        /// </summary>
        [Display(Name = "Partner Center")]
        [Required]
        public EndpointDetail PartnerCenterEndpoint { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether or not to synchronize Azure utilization records.
        /// </summary>
        [Required]
        public bool ProcessAzureUsage { get; set; }
    }
}