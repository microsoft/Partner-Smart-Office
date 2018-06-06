// -----------------------------------------------------------------------
// <copyright file="ProcessCustomerDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    public sealed class ProcessCustomerDetail
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
        /// Gets or sets the Partner Center endpoint.
        /// </summary>
        public EndpointDetail PartnerCenterEndpoint { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether or not to syncrhonize Azure utilization records.
        /// </summary>
        public bool ProcessAzureUsage { get; set; }
    }
}