// -----------------------------------------------------------------------
// <copyright file="AuditRecordCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.AuditRecords
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter;

    public class AuditRecordCollectionOperations : IAuditRecordCollectionOperations
    {
        private readonly PartnerServiceClient client;

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
                    new Uri($"/v1/auditrecords?startDate={startDate.ToString()}", UriKind.Relative), 
                    cancellationToken).ConfigureAwait(false);
        }
    }
}