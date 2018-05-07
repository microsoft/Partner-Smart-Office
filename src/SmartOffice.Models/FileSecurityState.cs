// -----------------------------------------------------------------------
// <copyright file="FileSecurityState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.Collections.Generic;

    public class FileSecurityState
    {
        public string AuthenticodeHash256 { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string RiskScore { get; set; }

        public string Sha256 { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }
    }
}