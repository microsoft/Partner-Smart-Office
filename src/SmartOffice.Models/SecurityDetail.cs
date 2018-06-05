// -----------------------------------------------------------------------
// <copyright file="SecurityDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    public sealed class SecurityDetail
    {
        /// <summary>
        /// Gets or sets the application endpoint.
        /// </summary>
        public EndpointDetail AppEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the customer details.
        /// </summary>
        public CustomerDetail Customer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating number of days of score results to retrieve starting from current date.
        /// </summary>
        public string Period { get; set; }
    }
}