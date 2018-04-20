// -----------------------------------------------------------------------
// <copyright file="HostSecurityState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.Collections.Generic;

    public class HostSecurityState
    {
        public string Fqdn { get; set; }

        public bool? IsAzureAadJoined { get; set; }

        public bool? IsAzureAadRegistered { get; set; }

        public bool? IsHybridAzureDomainJoined { get; set; }

        public string NetBiosName { get; set; }

        public string PrivateIpAddress { get; set; }

        public string PublicIpAddress { get; set; }

        public string RiskScore { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }
    }
}