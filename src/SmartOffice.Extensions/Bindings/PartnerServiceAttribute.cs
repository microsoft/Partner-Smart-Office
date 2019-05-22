// -----------------------------------------------------------------------
// <copyright file="PartnerServiceAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions
{
    using System;
    using Azure.WebJobs.Description;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class PartnerServiceAttribute : TokenBaseAttribute
    {
        /// <summary>
        /// Gets or sets the tenant identifer that owns the application.
        /// </summary>
        [AutoResolve]
        public string ApplicationTenantId { get; set; }

        /// <summary>
        /// Gets or sets the endpoint address for the Partner Center API.
        /// </summary>
        [AutoResolve]
        public string Endpoint { get; set; }
    }
}