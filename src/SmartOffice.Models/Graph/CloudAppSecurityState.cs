// -----------------------------------------------------------------------
// <copyright file="CloudAppSecurityState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CloudAppSecurityState
    {
        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }

        /// <summary>
        /// Gets ro sets the destination service IP address.
        /// </summary>
        [JsonProperty]
        public string DestinationServiceIp { get; set; }

        /// <summary>
        /// Gets or sets the destination service name.
        /// </summary>
        [JsonProperty]
        public string DestinationServiceName { get; set; }

        /// <summary>
        /// Gets or sets the risk score.
        /// </summary>
        [JsonProperty]
        public string RiskScore { get; set; }
    }
}