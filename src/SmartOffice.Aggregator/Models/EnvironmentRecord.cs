// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    /// <summary>
    /// Represents an environment record used for data aggregration.
    /// </summary>
    public sealed class EnvironmentRecord : DataEntry<EnvironmentEntry>
    {
        /// <summary>
        /// Gets or sets the end date for the audit record request.
        /// </summary>
        public string AuditEndDate { get; set; }

        /// <summary>
        /// Gets or sets the start date for the audit record request.
        /// </summary>
        public string AuditStartDate { get; set; }
    }
}