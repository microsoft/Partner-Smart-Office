// -----------------------------------------------------------------------
// <copyright file="ProcessCustomerDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.Collections.Generic;
    using Models.PartnerCenter.AuditRecords;

    public sealed class ProcessCustomerDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessCustomerDetail" /> class.
        /// </summary>
        public ProcessCustomerDetail()
        {
            AuditRecords = new List<AuditRecord>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessCustomerDetail" /> class.
        /// </summary>
        /// <param name="items">The collection whose elements are copied to the audit records list.</param>
        public ProcessCustomerDetail(IEnumerable<AuditRecord> items)
        {
            AuditRecords = new List<AuditRecord>(items);
        }

        /// <summary>
        /// Gets or sets the application endpoint.
        /// </summary>
        public EndpointDetail AppEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the audit records.
        /// </summary>
        public List<AuditRecord> AuditRecords { get; }

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