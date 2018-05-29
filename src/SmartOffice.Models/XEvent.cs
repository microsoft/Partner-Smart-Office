// -----------------------------------------------------------------------
// <copyright file="XEvent.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.Collections.Generic;
    using Models.PartnerCenter.AuditRecords;

    public sealed class XEvent
    {
        /// <summary>
        /// Gets or sets the application endpoint.
        /// </summary>
        public EndpointDetail AppEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the audit records.
        /// </summary>
        public List<AuditRecord> AuditRecords { get; set; }

        /// <summary>
        /// Gets or sets the customer details.
        /// </summary>
        public CustomerDetail Customer { get; set; }

        /// <summary>
        /// Gets or sets the Partner Center endpoint.
        /// </summary>
        public EndpointDetail PartnerCenterEndpoint { get; set; }
    }
}