// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a data entry.
    /// </summary>
    /// <typeparam name="TEntry">The type of data entry.</typeparam>
    public class DataEntry<TEntry>
        where TEntry : new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataEntry{TEntry}" /> class.
        /// </summary>
        public DataEntry()
        {
            Entry = new TEntry();
        }

        /// <summary>
        /// Gets the type of entry.
        /// </summary>
        public string EntryType { get; set; } = typeof(TEntry).IsGenericType && typeof(TEntry).GetGenericTypeDefinition() == typeof(List<>) ?
                typeof(TEntry).GetGenericArguments()[0].Name : typeof(TEntry).Name;

        /// <summary>
        /// Gets or sets the data entry.
        /// </summary>
        public TEntry Entry { get; set; }

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