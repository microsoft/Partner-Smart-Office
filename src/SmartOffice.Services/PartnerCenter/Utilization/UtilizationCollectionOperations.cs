// -----------------------------------------------------------------------
// <copyright file="UtilizationCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Utilization
{
    using System; 

    public class UtilizationCollectionOperations : IUtilizationCollectionOperations
    {
        private Lazy<IAzureUtilizationCollectionOperations> azureUtilizationOperations;

        public UtilizationCollectionOperations(PartnerServiceClient client, string customerId, string subscriptionId)
        {
            azureUtilizationOperations = new Lazy<IAzureUtilizationCollectionOperations>(
                () => new AzureUtilizationCollectionOperations(
                    client, customerId, subscriptionId));
        }

        public IAzureUtilizationCollectionOperations Azure => azureUtilizationOperations.Value;
    }
}