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
        [JsonProperty("activityGroupStates")]
        public IEnumerable<ActivityGroupState> ActivityGroupStates { get; set; }

        /// <summary>
        /// Gets or sets the application states.
        /// </summary>
        [JsonProperty("applicationStates")]
        public IEnumerable<ApplicationSecurityState> ApplicationStates { get; set; }

        /// <summary>
        /// Gets or sets the assigned resource for the alert.
        /// </summary>
        [JsonProperty("assignedTo")]
        public string AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the Azure subscription identifier.
        /// </summary>
        [JsonProperty("azureSubscriptionId")]
        public string AzureSubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the date and time the alert was close.
        /// </summary>
        public DateTimeOffset? ClosedDateTime { get; set; }

        /// <summary>
        /// Gets or sets any comments associated with the alert.
        /// </summary>
        public string Comments { get; set; }


        /// <summary>
        /// Gets or sets the date and time the alert was created.
        /// </summary>
        public DateTimeOffset? CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the description of the alert.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a collection identifier that caused the alert.
        /// </summary>
        public IEnumerable<string> DetectionIds { get; set; }

        /// <summary>
        /// Get or sets the date and time the event occurred. 
        /// </summary>
        public DateTimeOffset? EventDateTime { get; set; }

        /// <summary>
        /// Gets or sets the feedback associated with the alert.
        /// </summary>
        public AlertFeedback? Feedback { get; set; }

        /// <summary>
        /// Gets or sets the file security states.
        /// </summary>
        public IEnumerable<FileSecurityState> FileStates { get; set; }

        /// <summary>
        /// Gets or sets the host security states.
        /// </summary>
        [JsonProperty("hostStates")]
        public IEnumerable<HostSecurityState> HostStates { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the alert.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the date time and the alert was last modified.
        /// </summary>
        public DateTimeOffset? LastModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the malware states.
        /// </summary>
        public IEnumerable<MalwareState> MalwareStates { get; set; }

        /// <summary>
        /// Gets or sets a flag indiciating whether or not malware was running. 
        /// </summary>
        public bool? MalwareWasRunning { get; set; }

        /// <summary>
        /// Gets or sets a collection of network connections associated with the alert.
        /// </summary>
        public IEnumerable<NetworkConnection> NetworkConnections { get; set; }

        /// <summary>
        /// Gets or sets a collection of processes associated with the alert.
        /// </summary>
        public IEnumerable<Process> Processes { get; set; }

        /// <summary>
        /// Gets or sets a collection of recommended action for the alert.
        /// </summary>
        public IEnumerable<string> RecommendedActions { get; set; }

        /// <summary>
        /// Gets or sets the risk score for the alert.
        /// </summary>
        public string RiskScore { get; set; }

        /// <summary>
        /// Gets or sets a collection tags associated with the alert.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets the severity of the alert.
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Gets or sets a collection of source materials associated with the alert.
        /// </summary>
        public IEnumerable<string> SourceMaterials { get; set; }

        /// <summary>
        /// Gets or sets the status of the alert.
        /// </summary>
        public AlertStatus? Status { get; set; }

        /// <summary>
        /// Gets or sets the Azure Active Directory tenant identifier.
        /// </summary>
        public string AzureTenantId { get; set; }

        /// <summary>
        /// Gets or sets the title of the alert.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a collection of alert triggers.
        /// </summary>
        public IEnumerable<AlertTrigger> Triggers { get; set; }

        /// <summary>
        /// Gets or sets the type of alert.
        /// </summary>
        public AlertType? Type { get; set; }

        /// <summary>
        /// Gets or sets a collection of user security state associated with the alert.
        /// </summary>
        public IEnumerable<UserSecurityState> UserStates { get; set; }

        /// <summary>
        /// Gets or sets the vendor information for the alert.
        /// </summary>
        public SecurityVendorInformation VendorInformation { get; set; }

        /// <summary>
        /// Gets or sets a collection velnerability states for the alert.
        /// </summary>
        public IEnumerable<VulnerabilityState> VulnerabilityStates { get; set; }

        /// <summary>
        /// Gets or sets the OData type for the alert.
        /// </summary>
        public string ODataType { get; set; }

        /// <summary>
        /// Gets or sets the additional data associated with the alert.
        /// </summary>
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}