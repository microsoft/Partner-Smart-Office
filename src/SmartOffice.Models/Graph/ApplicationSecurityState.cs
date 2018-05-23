// -----------------------------------------------------------------------
// <copyright file="ApplicationSecurityState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System.Collections.Generic;

    public class ApplicationSecurityState
    {
        public string DeploymentPackageUrl { get; set; }

        public string Name { get; set; }

        public ApplicationPermissionsRequired? PermissionsRequired { get; set; }

        public string Publisher { get; set; }

        public string RiskScore { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }
    }
}
