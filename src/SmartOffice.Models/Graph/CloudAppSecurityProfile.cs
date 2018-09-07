// -----------------------------------------------------------------------
// <copyright file="CloudAppSecurityProfile.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CloudAppSecurityProfile
    {
        [JsonProperty]
        public string AzureSubscriptionId { get; set; }

        [JsonProperty]
        public string AzureTenantId { get; set; }

        [JsonProperty]
        public DateTimeOffset? CreatedDateTime { get; set; }

        [JsonProperty]
        public string DeploymentPackageUrl { get; set; }

        [JsonProperty]
        public string DestinationServiceName { get; set; }

        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public bool? IsSigned { get; set; }

        [JsonProperty]
        public DateTimeOffset? LastModifiedDateTime { get; set; }

        [JsonProperty]
        public string Manifest { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public ApplicationPermissionsRequired? PermissionsRequired { get; set; }

        [JsonProperty]
        public string Platform { get; set; }

        [JsonProperty]
        public string PolicyName { get; set; }

        [JsonProperty]
        public string Publisher { get; set; }

        [JsonProperty]
        public string RiskScore { get; set; }

        [JsonProperty]
        public IEnumerable<string> Tags { get; set; }

        [JsonProperty]
        public string Type { get; set; }

        [JsonProperty]
        public SecurityVendorInformation VendorInformation { get; set; }

        [JsonProperty]
        public string ODataType { get; set; }

        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}