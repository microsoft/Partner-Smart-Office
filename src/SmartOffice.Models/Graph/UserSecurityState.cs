// -----------------------------------------------------------------------
// <copyright file="UserSecurityState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class UserSecurityState
    {
        [JsonProperty]
        public string AccountName { get; set; }

        [JsonProperty]
        public string DomainName { get; set; }

        [JsonProperty]
        public EmailRole? EmailRole { get; set; }

        [JsonProperty]
        public bool? IsVpn { get; set; }

        [JsonProperty]
        public DateTimeOffset? LogonDateTime { get; set; }

        [JsonProperty]
        public string LogonId { get; set; }

        [JsonProperty]
        public string LogonIp { get; set; }

        [JsonProperty]
        public string LogonLocation { get; set; }

        [JsonProperty]
        public LogonType? LogonType { get; set; }

        [JsonProperty]
        public string OnPremisesSecurityIdentifier { get; set; }

        [JsonProperty]
        public string RiskScore { get; set; }

        [JsonProperty]
        public UserAccountSecurityType? UserAccountType { get; set; }

        [JsonProperty]
        public string UserPrincipalName { get; set; }

        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}