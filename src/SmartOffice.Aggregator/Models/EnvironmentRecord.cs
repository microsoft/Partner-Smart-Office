// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    /// <summary>
    /// Represents the details used to perform a delta or full sync of an environment.
    /// </summary>
    public sealed class EnvironmentRecord : EnvironmentEntry
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