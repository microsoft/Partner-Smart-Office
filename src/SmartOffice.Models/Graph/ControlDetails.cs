// -----------------------------------------------------------------------
// <copyright file="ControlDetails.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System;
    using Newtonsoft.Json;

    public class ControlDetails
    {
        /// <summary>
        /// Gets or sets the count for the control.
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public int? Count { get; set; }

        /// <summary>
        /// Gets or sets the expiry for the control.
        /// </summary>
        [JsonProperty(PropertyName = "expiry")]
        public int? Expiry { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether or not the control is active.
        /// </summary>
        [JsonProperty(PropertyName = "on")]
        public bool? On { get; set; }

        /// <summary>
        /// Gets or sets a flag indiciating whether or not the control is required.
        /// </summary>
        [JsonProperty(PropertyName = "required")]
        public bool? Required { get; set; }

        /// <summary>
        /// Gets or sets date when the control was last reviewed.
        /// </summary>
        [JsonProperty(PropertyName = "reviewed")]
        public DateTime? Reviewed { get; set; }

        /// <summary>
        /// Gets or sets the total for the control.
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public int? Total { get; set; }
    }
}
