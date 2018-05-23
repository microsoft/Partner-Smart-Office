// -----------------------------------------------------------------------
// <copyright file="ControlScore.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json;

    public class ControlScore
    {
        [JsonProperty(PropertyName = "controlDetails")]
        public ControlDetails ControlDetails { get; set; }

        /// <summary>
        /// Gets or sets the max score for the control score.
        /// </summary>
        [JsonProperty(PropertyName = "maxScore")]
        public int MaxScore { get; set; }

        /// <summary>
        /// Gets or sets the refernece identifier for the control score.
        /// </summary>
        [JsonProperty(PropertyName = "referenceId")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the score for the control score.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }
    }
}