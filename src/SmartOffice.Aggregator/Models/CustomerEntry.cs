// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    using System;

    /// <summary>
    /// Represents a customer entry.
    /// </summary>
    public class CustomerEntry : BaseDataEntry
    {
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