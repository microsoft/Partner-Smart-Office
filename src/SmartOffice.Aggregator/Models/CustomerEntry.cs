// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    using System;

    /// <summary>
    /// Represents a customer entry.
    /// </summary>
    public class CustomerEntry
    {
        /// <summary>
        /// Gets or sets the environment identifier associated with this entry.
        /// </summary>
        public string EnvironmentId { get; set; }

        /// <summary>
        /// Get or sets the environment name.
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the time last processed.
        /// </summary>
        public DateTimeOffset? LastProcessed { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}