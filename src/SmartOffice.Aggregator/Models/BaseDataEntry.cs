// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a denormalized data entry.
    /// </summary>
    public abstract class BaseDataEntry
    {
        /// <summary>
        /// Gets the type of entry.
        /// </summary>
        public string EntryType { get; set; }

        /// <summary>
        /// Gets or sets the environment identifier.
        /// </summary>
        public string EnvironmentId { get; set; }

        /// <summary>
        /// Gets or sets the environment name.
        /// </summary>
        public string EnvironmentName { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }
    }
}