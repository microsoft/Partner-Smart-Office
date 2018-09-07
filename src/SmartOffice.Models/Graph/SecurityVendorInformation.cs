// -----------------------------------------------------------------------
// <copyright file="SecurityVendorInformation.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class SecurityVendorInformation
    {
        [JsonProperty]
        public string Provider { get; set; }

        [JsonProperty]
        public string ProviderVersion { get; set; }

        [JsonProperty]
        public string SubProvider { get; set; }

        [JsonProperty]
        public string Vendor { get; set; }

        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}