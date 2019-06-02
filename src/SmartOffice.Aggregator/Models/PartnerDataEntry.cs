// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a de-normalized data entry.
    /// </summary>
    /// <typeparam name="TEntry">The type of data entry.</typeparam>
    public class PartnerDataEntry<TEntry> : BaseDataEntry
    {
        /// <summary>
        /// Gets or sets the entry.
        /// </summary>
        public TEntry Entry { get; set; }

        /// <summary>
        /// Gets the type of entry.
        /// </summary>
        public new string EntryType
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
    }
}