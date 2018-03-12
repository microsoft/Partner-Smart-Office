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
        /// <summary>
        /// Initializes an instance of the <see cref="StorageService" /> class.
        /// </summary>
        /// <param name="keyVaultEndpoint">The Azure Key Vault endpoint address.</param>
        /// <param name="connectionString">Name of the secret that contains the Azure Storage connection string.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
        Task InitializeAsync(string keyVaultEndpoint, string connectionString);

        /// <summary>
        /// Writes the specified entity to an Azure Storage queue.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to be written.</typeparam>
        /// <param name="queueName">Name of the queue where the entity will be written.</param>
        /// <param name="entity">Entity that will be written to the queue.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
        Task WriteToQueueAsync<TEntity>(string queueName, TEntity entity);
    }
}