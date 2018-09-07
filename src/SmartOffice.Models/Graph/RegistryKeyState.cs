// -----------------------------------------------------------------------
// <copyright file="RegistryKeyState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System.Collections.Generic;
    using Newtonsoft.Json; 

    public sealed class RegistryKeyState
    {
        [JsonProperty]
        public RegistryHive? Hive { get; set; }

        [JsonProperty]
        public string Key { get; set; }

        [JsonProperty]
        public string OldKey { get; set; }

        [JsonProperty]
        public string OldValueData { get; set; }

        [JsonProperty]
        public string OldValueName { get; set; }

        [JsonProperty]
        public RegistryOperation? Operation { get; set; }

        [JsonProperty]
        public int? ProcessId { get; set; }

        [JsonProperty]
        public string ValueData { get; set; }

        [JsonProperty]
        public string ValueName { get; set; }

        [JsonProperty]
        public RegistryValueType? ValueType { get; set; }

        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}