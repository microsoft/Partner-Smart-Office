// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models
{
    /// <summary>
    /// Represents the details used to perform a sync of a customer.
    /// </summary>
    public sealed class CustomerRecord : CustomerEntry
    {
        /// <summary>
        /// Gets or sets the application endpoint information.
        /// </summary>
        public EndpointEntry AppEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the number of days of Secure Score results to retrieve starting from current date.
        /// </summary>
        public int SecureScorePeriod { get; set; }
    }
}