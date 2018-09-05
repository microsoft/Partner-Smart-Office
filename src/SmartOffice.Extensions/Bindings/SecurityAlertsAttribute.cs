// -----------------------------------------------------------------------
// <copyright file="SecurityAlertsAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions.Bindings
{
    using System;
    using Azure.WebJobs.Description;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class SecurityAlertsAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the application identifier used to request an access token.
        /// </summary>
        [AutoResolve]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        [AutoResolve]
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the target resource that is the recipient of the token being requested.
        /// </summary>
        [AutoResolve]
        public string Resource { get; set; }

        /// <summary>
        /// Gets or sets the name of the Key Vault secret containing the application secret.
        /// </summary>
        [AutoResolve]
        public string SecretName { get; set; }
    }
}