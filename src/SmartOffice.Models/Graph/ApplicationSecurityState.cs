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
        /// <summary>
        /// Gets or sets the deployment package address.
        /// </summary>
        public string DeploymentPackageUrl { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the required application permissions.
        /// </summary>
        public ApplicationPermissionsRequired? PermissionsRequired { get; set; }

        /// <summary>
        /// Gets or sets the publisher. 
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the risk score.
        /// </summary>
        public string RiskScore { get; set; }

        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}
