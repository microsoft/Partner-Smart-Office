// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    /// <summary>
    /// Represents an environment entry.
    /// </summary>
    public class EnvironmentEntry
    {
        /// <summary>
        /// Gets or sets the application endpoint information.
        /// </summary>
        public EndpointEntry AppEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the Partner Center application endpoint information.
        /// </summary>
        public EndpointEntry PartnerCenterEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the Log Analytics workspace identifier. 
        /// </summary>
        public string WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the Azure Key Vault secret name for the Log Analytics workspace key.
        /// </summary>
        public string WorkspaceKeyName { get; set; }
    }
}