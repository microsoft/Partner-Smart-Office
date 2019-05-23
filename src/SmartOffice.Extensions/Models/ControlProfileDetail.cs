// -----------------------------------------------------------------------
// <copyright file="ControlProfileDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions
{
    using Models;

    public sealed class ControlProfileDetail
    {
        /// <summary>
        /// Gets or sets the application endpoint.
        /// </summary>
        public EndpointDetail AppEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the customer details.
        /// </summary>
        public CustomerDetail Customer { get; set; }
    }
}