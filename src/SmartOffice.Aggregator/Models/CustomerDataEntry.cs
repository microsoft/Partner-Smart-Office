// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    /// <summary>
    /// Represents a data entry owned by a customer.
    /// </summary>
    /// <typeparam name="TEntry"></typeparam>
    public class CustomerDataEntry<TEntry> : DataEntry<TEntry>
        where TEntry : new()
    {
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer name.
        /// </summary>
        public string CustomerName { get; set; }
    }
}