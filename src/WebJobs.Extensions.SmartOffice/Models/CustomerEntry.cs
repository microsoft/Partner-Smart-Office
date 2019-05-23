// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.WebJobs.Extensions.SmartOffice
{
    using Graph;

    /// <summary>
    /// Represents a customer entry that will be presisted.
    /// </summary>
    public sealed class CustomerEntry
    {
        /// <summary>
        /// Gets or set the customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or set the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Secure Score information.
        /// </summary>
        public SecureScore SecureScore { get; set; }
    }
}