// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.WebJobs.Extensions.SmartOffice.Models
{
    using System.Collections.Generic;
    using Graph;

    /// <summary>
    /// Represents a customer entry that will be presisted.
    /// </summary>
    public sealed class CustomerEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerEntry" /> class.
        /// </summary>
        public CustomerEntry()
        {
            SecureScoreControlProfiles = new List<SecureScoreControlProfile>();
            Subscriptions = new List<SubscriptionEntry>();
        }

        /// <summary>
        /// Gets or set the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets a collection of Secure Score control profiles.
        /// </summary>
        public List<SecureScoreControlProfile> SecureScoreControlProfiles { get; private set; }

        /// <summary>
        /// Gets a collection of subscriptions.
        /// </summary>
        public List<SubscriptionEntry> Subscriptions { get; private set; }
    }
}