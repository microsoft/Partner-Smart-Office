// -----------------------------------------------------------------------
// <copyright file="CustomerDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System;
    using System.Collections.Generic;
    using Models.PartnerCenter;

    public class CustomerDetail
    {
        public EndpointDetail AppEndpoint { get; set; }

        public List<AuditRecord> AuditRecords { get; set; }

        public string Id { get; set; }

        public DateTimeOffset? LastProcessed { get; set; }

        public EndpointDetail PartnerCenterEndpoint { get; set; }
    }
}
