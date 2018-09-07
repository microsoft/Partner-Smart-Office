// -----------------------------------------------------------------------
// <copyright file="Process.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Process
    {
        [JsonProperty]
        public string AccountName { get; set; }

        [JsonProperty]
        public string CommandLine { get; set; }

        [JsonProperty]
        public DateTimeOffset? CreatedDateTime { get; set; }

        [JsonProperty]
        public FileHash FileHash { get; set; }

        [JsonProperty]
        public ProcessIntegrityLevel? IntegrityLevel { get; set; }

        [JsonProperty]
        public bool? IsElevated { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonProperty]
        public DateTimeOffset? ParentProcessCreatedDateTime { get; set; }

        [JsonProperty]
        public int? ParentProcessId { get; set; }

        [JsonProperty]
        public string ParentProcessName { get; set; }

        [JsonProperty]
        public string Path { get; set; }

        [JsonProperty]
        public int? ProcessId { get; set; }

        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}