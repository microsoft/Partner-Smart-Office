// -----------------------------------------------------------------------
// <copyright file="ControlDetails.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System;
    using Newtonsoft.Json;

    public class ControlDetails
    {
        [JsonProperty(PropertyName = "count")]
        public int? Count { get; set; }

        [JsonProperty(PropertyName = "expiry")]
        public int? Expiry { get; set; }

        [JsonProperty(PropertyName = "on")]
        public bool? On { get; set; }

        [JsonProperty(PropertyName = "required")]
        public bool? Required { get; set; }

        [JsonProperty(PropertyName = "reviewed")]
        public DateTime? Reviewed { get; set; }

        [JsonProperty(PropertyName = "total")]
        public int? Total { get; set; }
    }
}
