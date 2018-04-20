using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Partner.SmartOffice.Models
{
    public class UserSecurityState
    {
        public string AccountName { get; set; }

        public string DomainName { get; set; }

        public DateTimeOffset? LogonDateTime { get; set; }

        public string LogonId { get; set; }

        public string LogonIpAddress { get; set; }

        public string LogonLocation { get; set; }

        public LogonType? LogonType { get; set; }

        public string OnPremisesSecurityIdentifier { get; set; }

        public string RiskScore { get; set; }

        public UserAccountSecurityType? UserAccountType { get; set; }

        public string UserPrincipalName { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }
    }
}