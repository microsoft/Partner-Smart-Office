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

        public async Task<SeekBasedResourceCollection<AuditRecord>> QueryAsync(Link nextLink, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await client.GetAsync<SeekBasedResourceCollection<AuditRecord>>(
                nextLink,
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<SeekBasedResourceCollection<AuditRecord>> QueryAsync(DateTime startDate, DateTime? endDate = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await client.GetAsync<SeekBasedResourceCollection<AuditRecord>>(
                new Uri($"/v1/auditrecords?startDate={startDate.ToString(CultureInfo.InvariantCulture)}", UriKind.Relative),
                cancellationToken).ConfigureAwait(false);
        }
    }
}