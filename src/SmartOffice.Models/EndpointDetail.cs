// -----------------------------------------------------------------------
// <copyright file="EndpointDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System;

    public class EndpointDetail
    {
        /// <summary>
        /// Gets or sets identifier of the application.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the application secret.
        /// </summary>
        public string ApplicationSecretId { get; set; }

        /// <summary>
        /// Gets or sets the endpoint address.
        /// </summary>
        public string EndpointUri { get; set; }

        /// <summary>
        /// Gets or set the identifier of the tenant.
        /// </summary>
        public string TenantId { get; set; }
    }
}
