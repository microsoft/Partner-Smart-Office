// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.WebJobs.Extensions.SmartOffice
{
    using System;
    using Azure.WebJobs.Description;

    /// <summary>
    /// Represents the base attribute for operations that require authentication.
    /// </summary>
    public abstract class TokenBaseAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the application identifier used to request an access token.
        /// </summary>
        [AutoResolve]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the application secret used to request an access token.
        /// </summary>
        [AutoResolve]
        public string ApplicationSecret { get; set; }

        /// <summary>
        /// Gets or sets Azure AD tenant identifier.
        /// </summary>
        [AutoResolve]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the target resource that is the recipient of the token being requested.
        /// </summary>
        [AutoResolve]
        public string Resource { get; set; } = "https://graph.microsoft.com";
    }
}