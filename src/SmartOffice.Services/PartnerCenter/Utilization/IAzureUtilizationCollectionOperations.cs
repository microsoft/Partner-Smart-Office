// -----------------------------------------------------------------------
// <copyright file="IAzureUtilizationCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Utilization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter;
    using Models.PartnerCenter.Utilizations;

    public interface IAzureUtilizationCollectionOperations
    {
        Task<ResourceCollection<AzureUtilizationRecord>> QueryAsync(
            DateTimeOffset startTime, 
            DateTimeOffset endTime, 
            AzureUtilizationGranularity granularity = AzureUtilizationGranularity.Daily, 
            bool showDetails = true, 
            int size = 1000,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ResourceCollection<AzureUtilizationRecord>> QueryAsync(
            Link nextLink,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}