// -----------------------------------------------------------------------
// <copyright file="IAuditRecordCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.AuditRecords
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter;

    public interface IAuditRecordCollectionOperations
    {
        Task<SeekBasedResourceCollection<AuditRecord>> QueryAsync(Link nextLink, CancellationToken cancellationToken = default(CancellationToken));

        Task<SeekBasedResourceCollection<AuditRecord>> QueryAsync(DateTime startDate, DateTime? endDate = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}