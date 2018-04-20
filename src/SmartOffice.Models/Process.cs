// -----------------------------------------------------------------------
// <copyright file="Process.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System;
    using System.Collections.Generic;

    public class Process
    {
        public string AuthenticodeHash256 { get; set; }

        public string CommandLine { get; set; }

        public DateTimeOffset? CreatedDateTime { get; set; }

        public ProcessIntegrityLevel? IntegrityLevel { get; set; }

        public bool? IsElevated { get; set; }

        public string Name { get; set; }

        public DateTimeOffset? ParentProcessCreatedDateTime { get; set; }

        public int? ParentProcessId { get; set; }

        public string ParentProcessName { get; set; }

        public string Path { get; set; }

        public int? ProcessId { get; set; }

        public string Sha256 { get; set; }

        public string UserAccount { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }
    }
}