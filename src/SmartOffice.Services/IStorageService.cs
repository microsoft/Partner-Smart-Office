// -----------------------------------------------------------------------
// <copyright file="IStorageService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System.Threading.Tasks;

    public interface IStorageService
    {
        Task InitializeAsync(string keyVaultEndpoint, string connectionString);

        Task WriteToQueueAsync<TEntity>(string queueName, TEntity entity);
    }
}