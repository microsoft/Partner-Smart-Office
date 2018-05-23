// -----------------------------------------------------------------------
// <copyright file="EnvironmentDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using Newtonsoft.Json;

    public class EnvironmentDetail
    {
        /// <summary>
        /// Gets or sets the application endpoint details for the environment.
        /// </summary>
        public EndpointDetail AppEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the type for the environment.
        /// </summary>
        public EnvironmentType EnvironmentType { get; set; }

        /// <summary>
        /// Gets or sets the friendly name for the environment.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the environment.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Microsoft Partner Center endpoint details for the environment.
        /// </summary>
        public EndpointDetail PartnerCenterEndpoint { get; set; }
    }
}