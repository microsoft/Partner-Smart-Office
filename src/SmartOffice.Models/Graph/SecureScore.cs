// -----------------------------------------------------------------------
// <copyright file="SecureScore.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System; 
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class SecureScore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SecureScore" /> class.
        /// </summary>
        public SecureScore()
        {
            ControlScores = new List<ControlScore>();
            EnabledServices = new List<string>();
        }

        /// <summary>
        /// Gets or sets the secure score for account controls.
        /// </summary>
        [JsonProperty(PropertyName = "accountScore")]
        public int AccountScore { get; set; }

        /// <summary>
        /// Gets or sets the number of active users.
        /// </summary>
        [JsonProperty(PropertyName = "activeUserCount")]
        public int ActiveUserCount { get; set; }

        /// <summary>
        /// Gets or sets the average secure score for account controls.
        /// </summary>
        [JsonProperty(PropertyName = "averageAccountScore")]
        public decimal AverageAccountScore { get; set; }

        /// <summary>
        /// Gets or sets the average secure score for data controls.
        /// </summary>
        [JsonProperty(PropertyName = "averageDataScore")]
        public decimal AverageDataScore { get; set; }

        /// <summary>
        /// Gets or sets the average secure score for device controls.
        /// </summary>
        [JsonProperty(PropertyName = "averageDeviceScore")]
        public decimal AverageDeviceScore { get; set; }

        /// <summary>
        /// Gets the or sets the maximum attainable secure score.
        /// </summary>
        [JsonProperty(PropertyName = "averageMaxSecureScore")]
        public decimal AverageMaxSecureScore { get; set; }

        /// <summary>
        /// Gets or sets the average secure score.
        /// </summary>
        [JsonProperty(PropertyName = "averageSecureScore")]
        public decimal AverageSecureScore { get; set; }

        /// <summary>
        /// Gets or sets the collection of control scores.
        /// </summary>
        [JsonProperty(PropertyName = "controlScores")]
        public List<ControlScore> ControlScores { get; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the secure score for data controls. 
        /// </summary>
        [JsonProperty(PropertyName = "dataScore")]
        public int DataScore { get; set; }

        /// <summary>
        /// Gets or stes the secure score for device controls.
        /// </summary>
        [JsonProperty(PropertyName = "deviceScore")]
        public int DeviceScore { get; set; }

        /// <summary>
        /// Gets or sets the list of services enabled for the tenant.
        /// </summary>
        [JsonProperty(PropertyName = "enabledServices")]
        public List<string> EnabledServices { get; }

        [JsonProperty(PropertyName = "id")]
        public string Id => $"{CreatedDate.Month}-{CreatedDate.Day}-{CreatedDate.Year}-{TenantId}";

        /// <summary>
        /// Gets or sets the number of licensed users.
        /// </summary>
        [JsonProperty(PropertyName = "licensedUserCount")]
        public int LicensedUserCount { get; set; }

        /// <summary>
        /// Gets or sets the maximum attainable secure score.
        /// </summary>
        [JsonProperty(PropertyName = "maxSecureScore")]
        public int MaxSecureScore { get; set; }

        /// <summary>
        /// Gets or sets the secure score value.
        /// </summary>
        [JsonProperty(PropertyName = "secureScore")]
        public int Score { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the tenant that owns this secure score.
        /// </summary>
        [JsonProperty(PropertyName = "tenantId")]
        public string TenantId { get; set; }
    }
}