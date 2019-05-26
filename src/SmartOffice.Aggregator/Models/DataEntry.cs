// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a denormalized data entry.
    /// </summary>
    /// <typeparam name="TEntry">The type of data entry.</typeparam>
    public class DataEntry<TEntry>
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer name.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        public TEntry Entry { get; set; }

        /// <summary>
        /// Gets the type of entry.
        /// </summary>
        public string EntryType
        {
            get
            {
                if (typeof(TEntry).IsGenericType && typeof(TEntry).GetGenericTypeDefinition() == typeof(List<>))
                {
                    return typeof(TEntry).GetGenericArguments()[0].Name;
                }

                return typeof(TEntry).Name;
            }
        }

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