// -----------------------------------------------------------------------
// <copyright file="SecureScoreAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings
{
    using System;
    using Azure.WebJobs;
    using Azure.WebJobs.Description;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class SecureScoreAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the application identifier used to request an access token.
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
        [AppSetting(Default = "ApplicationId")]
#pragma warning restore CS0618 // Type or member is obsolete
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
        [AutoResolve]
#pragma warning restore CS0618 // Type or member is obsolete
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the period for the secure score.
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the target resource that is the recipient of the token being requested.
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// Gets or sets the name of the Key Vault secret containing the application secret.
        /// </summary>
        public string SecretName { get; set; }
    }
}