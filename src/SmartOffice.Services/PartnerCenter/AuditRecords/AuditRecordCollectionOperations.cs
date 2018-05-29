// -----------------------------------------------------------------------
// <copyright file="AuditRecordCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.AuditRecords
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter;
    using Models.PartnerCenter.AuditRecords;

    public class AuditRecordCollectionOperations : IAuditRecordCollectionOperations
    {
        /// <summary>
        /// Provides the ability to perform HTTP operations.
        /// </summary>
        private readonly PartnerServiceClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditRecordCollectionOperations" /> class.
        /// </summary>
        /// <param name="client">Provides the ability to perform HTTP operations.</param>
        public AuditRecordCollectionOperations(PartnerServiceClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Queries audit records associated to the partner.
        /// </summary>
        /// <param name="nextLink">An instance of the <see cref="Link" /> class that represents the next page to be requested.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The audit records that match the given query.</returns>
        public async Task<SeekBasedResourceCollection<AuditRecord>> QueryAsync(Link nextLink, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await client.GetAsync<SeekBasedResourceCollection<AuditRecord>>(
                nextLink,
                cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Queries audit records associated to the partner.
        /// </summary>
        /// <param name="startDate">The start date of the audit record logs.</param>
        /// <param name="endDate">The end date of the audit record logs</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The audit records that match the given query.</returns>
        public async Task<SeekBasedResourceCollection<AuditRecord>> QueryAsync(DateTime startDate, DateTime? endDate = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await client.GetAsync<SeekBasedResourceCollection<AuditRecord>>(
                new Uri($"/v1/auditrecords?startDate={startDate.ToString(CultureInfo.InvariantCulture)}", UriKind.Relative),
                cancellationToken).ConfigureAwait(false);
        }
    }
}