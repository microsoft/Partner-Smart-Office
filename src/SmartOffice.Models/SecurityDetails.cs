// -----------------------------------------------------------------------
// <copyright file="SecurityDetails.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    public sealed class SecurityDetails
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