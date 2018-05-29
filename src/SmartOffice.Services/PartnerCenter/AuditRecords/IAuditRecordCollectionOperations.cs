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
    using Models.PartnerCenter.AuditRecords;

    public interface IAuditRecordCollectionOperations
    {
        /// <summary>
        /// Queries audit records associated to the partner.
        /// </summary>
        /// <param name="nextLink">An instance of the <see cref="Link" /> class that represents the next page to be requested.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The audit records that match the given query.</returns>
        Task<SeekBasedResourceCollection<AuditRecord>> QueryAsync(Link nextLink, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Queries audit records associated to the partner.
        /// </summary>
        /// <param name="startDate">The start date of the audit record logs.</param>
        /// <param name="endDate">The end date of the audit record logs</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The audit records that match the given query.</returns>
        Task<SeekBasedResourceCollection<AuditRecord>> QueryAsync(DateTime startDate, DateTime? endDate = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}