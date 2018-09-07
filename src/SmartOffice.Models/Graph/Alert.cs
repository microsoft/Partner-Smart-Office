// -----------------------------------------------------------------------
// <copyright file="Alert.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Alert
    {
        /// <summary>
        /// Gets or sets the activity group states.
        /// </summary>
        [JsonProperty]
        public string ActivityGroupName { get; set; }

        /// <summary>
        /// Gets or sets the assigned resource for the alert.
        /// </summary>
        [JsonProperty]
        public string AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the Azure subscription identifier.
        /// </summary>
        [JsonProperty]
        public string AzureSubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the Azure tenant identifier.
        /// </summary>
        [JsonProperty]
        public string AzureTenantId { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        [JsonProperty]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the date and time the alert was close.
        /// </summary>
        [JsonProperty]
        public DateTimeOffset? ClosedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the cloud application states.
        /// </summary>
        [JsonProperty]
        public IEnumerable<CloudAppSecurityState> CloudAppStates { get; set; }

        /// <summary>
        /// Gets or sets any comments associated with the alert.
        /// </summary>
        [JsonProperty]
        public string Comments { get; set; }

        /// <summary>
        /// Gets ro sets the confidence.
        /// </summary>
        [JsonProperty]
        public int? Confidence { get; set; }

        /// <summary>
        /// Gets or sets the date and time the alert was created.
        /// </summary>
        [JsonProperty]
        public DateTimeOffset? CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the description of the alert.
        /// </summary>
        [JsonProperty]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a collection identifier that caused the alert.
        /// </summary>
        [JsonProperty]
        public IEnumerable<string> DetectionIds { get; set; }

        /// <summary>
        /// Get or sets the date and time the event occurred. 
        /// </summary>
        [JsonProperty]
        public DateTimeOffset? EventDateTime { get; set; }

        /// <summary>
        /// Gets or sets the feedback associated with the alert.
        /// </summary>
        [JsonProperty]
        public AlertFeedback? Feedback { get; set; }

        /// <summary>
        /// Gets or sets the file security states.
        /// </summary>
        [JsonProperty]
        public IEnumerable<FileSecurityState> FileStates { get; set; }

        /// <summary>
        /// Gets or sets the host security states.
        /// </summary>
        [JsonProperty]
        public IEnumerable<HostSecurityState> HostStates { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the alert.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the date time and the alert was last modified.
        /// </summary>
        [JsonProperty]
        public DateTimeOffset? LastModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the malware states.
        /// </summary>
        [JsonProperty]
        public IEnumerable<MalwareState> MalwareStates { get; set; }

        /// <summary>
        /// Gets or sets a collection of network connections associated with the alert.
        /// </summary>
        [JsonProperty]
        public IEnumerable<NetworkConnection> NetworkConnections { get; set; }

        /// <summary>
        /// Gets or sets a collection of processes associated with the alert.
        /// </summary>
        [JsonProperty]
        public IEnumerable<Process> Processes { get; set; }

        /// <summary>
        /// Gets or sets a collection of recommended action for the alert.
        /// </summary>
        [JsonProperty]
        public IEnumerable<string> RecommendedActions { get; set; }

        /// <summary>
        /// Gets or sets the registry key states.
        /// </summary>
        [JsonProperty]
        public IEnumerable<RegistryKeyState> RegistryKeyStates { get; set; }

        /// <summary>
        /// Gets or sets the severity of the alert.
        /// </summary>
        [JsonProperty]
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets the source materials.
        /// </summary>
        [JsonProperty]
        public IEnumerable<string> SourceMaterials { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [JsonProperty]
        public AlertStatus? Status { get; set; }

        /// <summary>
        /// Gets or sets a collection tags associated with the alert.
        /// </summary>
        [JsonProperty]
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the title of the alert.
        /// </summary>
        [JsonProperty]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a collection of alert triggers.
        /// </summary>
        [JsonProperty]
        public IEnumerable<AlertTrigger> Triggers { get; set; }

        /// <summary>
        /// Gets or sets a collection of user security state associated with the alert.
        /// </summary>
        [JsonProperty]
        public IEnumerable<UserSecurityState> UserStates { get; set; }

        /// <summary>
        /// Gets or sets the vendor information for the alert.
        /// </summary>
        [JsonProperty]
        public SecurityVendorInformation VendorInformation { get; set; }

        /// <summary>
        /// Gets or sets a collection velnerability states for the alert.
        /// </summary>
        [JsonProperty]
        public IEnumerable<VulnerabilityState> VulnerabilityStates { get; set; }

        /// <summary>
        /// Gets or sets the OData type for the alert.
        /// </summary>
        [JsonProperty]
        public string ODataType { get; set; }

        /// <summary>
        /// Gets or sets the additional data associated with the alert.
        /// </summary>
        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}