// -----------------------------------------------------------------------
// <copyright file="IUtilizationCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Utilization
{
    public interface IUtilizationCollectionOperations
    {
        IAzureUtilizationCollectionOperations Azure { get; }
    }
}