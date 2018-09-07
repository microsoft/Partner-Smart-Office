﻿// -----------------------------------------------------------------------
// <copyright file="NetworkConnection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System.Collections.Generic;
    using Newtonsoft.Json; 

    public class NetworkConnection
    {
        [JsonProperty]
        public string Category { get; set; }

        [JsonProperty]
        public string Family { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public string Severity { get; set; }

        [JsonProperty]
        public bool? WasRunning { get; set; }

        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}
