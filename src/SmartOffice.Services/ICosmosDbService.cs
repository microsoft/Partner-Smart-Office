// -----------------------------------------------------------------------
// <copyright file="ICosmosDbService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Microsoft.Partner.SmartOffice.Services
{
    using System.Threading.Tasks; 

    public interface ICosmosDbService
    {
        Task InitializeAsync(CosmosDbOptions options);

        Task ExecuteStoredProcedureAsync(string databaseId, string collectionId, string storedProcedureId, params dynamic[] procedureParams);
    }
}