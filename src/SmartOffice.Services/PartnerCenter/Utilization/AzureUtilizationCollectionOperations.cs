// -----------------------------------------------------------------------
// <copyright file="AzureUtilizationCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Utilization
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter;
    using Models.PartnerCenter.Utilizations;

    public class AzureUtilizationCollectionOperations : IAzureUtilizationCollectionOperations
    {
        private readonly PartnerServiceClient client;

        private readonly string customerId;

        private readonly string subscriptionId;

        public AzureUtilizationCollectionOperations(PartnerServiceClient client, string customerId, string subscriptionId)
        {
            this.client = client;
            this.customerId = customerId;
            this.subscriptionId = subscriptionId;
        }

        public async Task<ResourceCollection<AzureUtilizationRecord>> QueryAsync(
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            AzureUtilizationGranularity granularity = AzureUtilizationGranularity.Daily,
            bool showDetails = true,
            int size = 1000,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            return await client.GetAsync<ResourceCollection<AzureUtilizationRecord>>(
                new Uri($"/v1/customers/{customerId}/subscriptions/{subscriptionId}/utilizations/azure?start_time={startTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}&end_time={endTime.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}&granularity={granularity.ToString()}&show_details={showDetails.ToString(CultureInfo.InvariantCulture)}&size={size.ToString(CultureInfo.InvariantCulture)}",
                    UriKind.Relative),
                cancellationToken).ConfigureAwait(false);
        }

        public async Task<ResourceCollection<AzureUtilizationRecord>> QueryAsync(
            Link nextLink, 
            CancellationToken cancellationToken = default(CancellationToken))
        {

            return await client.GetAsync<ResourceCollection<AzureUtilizationRecord>>(
                nextLink,
                cancellationToken).ConfigureAwait(false);
        }
    }
}