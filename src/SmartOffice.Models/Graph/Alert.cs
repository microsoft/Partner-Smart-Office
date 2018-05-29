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
        [JsonProperty("activityGroupStates")]
        public IEnumerable<ActivityGroupState> ActivityGroupStates { get; set; }

        [JsonProperty("applicationStates")]
        public IEnumerable<ApplicationSecurityState> ApplicationStates { get; set; }

        [JsonProperty("assignedTo")]
        public string AssignedTo { get; set; }

        [JsonProperty("azureSubscriptionId")]
        public string AzureSubscriptionId { get; set; }

        public string Category { get; set; }

        public DateTimeOffset? ClosedDateTime { get; set; }

        public string Comments { get; set; }

        public DateTimeOffset? CreatedDateTime { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> DetectionIds { get; set; }

        public DateTimeOffset? EventDateTime { get; set; }

        public AlertFeedback? Feedback { get; set; }

        public IEnumerable<FileSecurityState> FileStates { get; set; }

        [JsonProperty("hostStates")]
        public IEnumerable<HostSecurityState> HostStates { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public DateTimeOffset? LastModifiedDateTime { get; set; }

        public IEnumerable<MalwareState> MalwareStates { get; set; }

        public bool? MalwareWasRunning { get; set; }

        public IEnumerable<NetworkConnection> NetworkConnections { get; set; }

        public IEnumerable<Process> Processes { get; set; }

        public IEnumerable<string> RecommendedActions { get; set; }

        public string RiskScore { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string Severity { get; set; }

        public IEnumerable<string> SourceMaterials { get; set; }

        public AlertStatus? Status { get; set; }

        public string AzureTenantId { get; set; }

        public string Title { get; set; }

        public IEnumerable<AlertTrigger> Triggers { get; set; }

        public AlertType? Type { get; set; }

        public IEnumerable<UserSecurityState> UserStates { get; set; }

        public SecurityVendorInformation VendorInformation { get; set; }

        public IEnumerable<VulnerabilityState> VulnerabilityStates { get; set; }

        public string ODataType { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }
    }
}