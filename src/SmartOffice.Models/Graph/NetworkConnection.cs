// -----------------------------------------------------------------------
// <copyright file="NetworkConnection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json; 

    public class NetworkConnection
    {
        [JsonProperty]
        public string ApplicationName { get; set; }

        [JsonProperty]
        public string DestinationAddress { get; set; }

        [JsonProperty]
        public string DestinationDomain { get; set; }

        [JsonProperty]
        public string DestinationPort { get; set; }

        [JsonProperty]
        public string DestinationUrl { get; set; }

        [JsonProperty]
        public ConnectionDirection? Direction { get; set; }

        [JsonProperty]
        public DateTimeOffset? DomainRegisteredDateTime { get; set; }

        [JsonProperty]
        public string LocalDnsName { get; set; }

        [JsonProperty]
        public string NatDestinationAddress { get; set; }

        [JsonProperty]
        public string NatDestinationPort { get; set; }

        [JsonProperty]
        public string NatSourceAddress { get; set; }

        [JsonProperty]
        public string NatSourcePort { get; set; }

        [JsonProperty]
        public SecurityNetworkProtocol? Protocol { get; set; }

        [JsonProperty]
        public string RiskScore { get; set; }

        [JsonProperty]
        public string SourceAddress { get; set; }

        [JsonProperty]
        public string SourcePort { get; set; }

        [JsonProperty]
        public ConnectionStatus? Status { get; set; }

        [JsonProperty]
        public string UrlParameters { get; set; }

        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}
