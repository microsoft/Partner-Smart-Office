// -----------------------------------------------------------------------
// <copyright file="FileSecurityState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class FileSecurityState
    {
        [JsonProperty]
        public FileHash FileHash { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Path { get; set; }

        [JsonProperty]
        public string RiskScore { get; set; }

        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}