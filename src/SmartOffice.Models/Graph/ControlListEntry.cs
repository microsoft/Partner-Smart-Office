// -----------------------------------------------------------------------
// <copyright file="ControlListEntry.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents an entry from the Secure Score control list.
    /// </summary>
    public class ControlListEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControlListEntry" /> class.
        /// </summary>
        public ControlListEntry()
        {
            Threats = new List<string>();
        }

        /// <summary>
        /// Gets or sets the action category for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "actionCategory")]
        public string ActionCategory { get; set; }

        /// <summary>
        /// Gets or sets the action URL for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "actionUrl")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// Gets or sets the baseline for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "baseline")]
        public int Baseline { get; set; }

        /// <summary>
        // Gets or sets a value that indicates whether or not the entry is deprecated.
        /// </summary>
        [JsonProperty(PropertyName = "deprecated")]
        public bool Deprecated { get; set; }

        /// <summary>
        /// Gets or sets the description for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the enablement for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "enablement")]
        public string Enablement { get; set; }

        /// <summary>
        /// Gets or sets the implementation cost for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "implementationCost")]
        public string ImplementationCost { get; set; }

        /// <summary>
        /// Gets or sets the name for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the remediation change for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "remediationChange")]
        public string RemediationChange { get; set; }

        /// <summary>
        /// Gets or sets the rediation impact for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "remediationImpact")]
        public string RemediationImpact { get; set; }

        /// <summary>
        /// Gets or sets the stack value for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "stackRank")]
        public int StackRank { get; set; }

        /// <summary>
        /// Gets or sets the list of threats associated with the entry.
        /// </summary>
        [JsonProperty(PropertyName = "threats")]
        public List<string> Threats { get; }

        /// <summary>
        /// Gets or sets the tier for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "tier")]
        public string Tier { get; set; }

        /// <summary>
        /// Gets or sets the user impact for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "userImpact")]
        public string UserImpact { get; set; }

        /// <summary>
        /// Gets or sets the workload for the entry.
        /// </summary>
        [JsonProperty(PropertyName = "workload")]
        public string Workload { get; set; }
    }
}