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
        [JsonProperty]
        public string ActivityGroupName { get; set; }

        [JsonProperty]
        public string AssignedTo { get; set; }

        [JsonProperty]
        public string AzureSubscriptionId { get; set; }

        [JsonProperty]
        public string AzureTenantId { get; set; }

        [JsonProperty]
        public string Category { get; set; }

        [JsonProperty]
        public DateTimeOffset? ClosedDateTime { get; set; }

        [JsonProperty]
        public IEnumerable<CloudAppSecurityState> CloudAppStates { get; set; }

        [JsonProperty]
        public IEnumerable<string> Comments { get; set; }

        [JsonProperty]
        public int? Confidence { get; set; }

        [JsonProperty]
        public DateTimeOffset? CreatedDateTime { get; set; }

        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public IEnumerable<string> DetectionIds { get; set; }

        [JsonProperty]
        public DateTimeOffset? EventDateTime { get; set; }

        [JsonProperty]
        public AlertFeedback? Feedback { get; set; }

        [JsonProperty]
        public IEnumerable<FileSecurityState> FileStates { get; set; }

        [JsonProperty]
        public IEnumerable<HostSecurityState> HostStates { get; set; }

        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public DateTimeOffset? LastModifiedDateTime { get; set; }

        [JsonProperty]
        public IEnumerable<MalwareState> MalwareStates { get; set; }

        [JsonProperty]
        public IEnumerable<NetworkConnection> NetworkConnections { get; set; }

        [JsonProperty]
        public IEnumerable<Process> Processes { get; set; }

        [JsonProperty]
        public IEnumerable<string> RecommendedActions { get; set; }

        [JsonProperty]
        public IEnumerable<RegistryKeyState> RegistryKeyStates { get; set; }

        [JsonProperty]
        public string Severity { get; set; }

        [JsonProperty]
        public IEnumerable<string> SourceMaterials { get; set; }

        [JsonProperty]
        public AlertStatus? Status { get; set; }

        [JsonProperty]
        public IEnumerable<string> Tags { get; set; }

        [JsonProperty]
        public string Title { get; set; }

        [JsonProperty]
        public IEnumerable<AlertTrigger> Triggers { get; set; }

        [JsonProperty]
        public IEnumerable<UserSecurityState> UserStates { get; set; }

        [JsonProperty]
        public SecurityVendorInformation VendorInformation { get; set; }

        [JsonProperty]
        public IEnumerable<VulnerabilityState> VulnerabilityStates { get; set; }

        [JsonProperty]
        public string ODataType { get; set; }

        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}