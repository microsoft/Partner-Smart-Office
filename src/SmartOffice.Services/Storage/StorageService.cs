// -----------------------------------------------------------------------
// <copyright file="StorageService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Storage
{
    using System;
    using System.Threading.Tasks;
    using KeyVault;
    using Newtonsoft.Json;
    using WindowsAzure.Storage;
    using WindowsAzure.Storage.Queue;

    public class StorageService : IStorageService
    {
        /// <summary>
        /// Name of the storage account connection string secret.
        /// </summary>
        private const string StorageAccountConnectionStringName = "StorageConnectionString";

        /// <summary>
        /// Provides the ability to interact with Azure Storage.
        /// </summary>
        private static CloudStorageAccount storageAccount;

        /// <summary>
        /// Singleton instance of the <see cref="StorageService" /> class.
        /// </summary>
        private static readonly Lazy<StorageService> instance = new Lazy<StorageService>(() => new StorageService());

        /// <summary>
        /// Gets an instance of the <see cref="StorageService" /> class.
        /// </summary>
        public static StorageService Instance => instance.Value;

        /// <summary>
        /// Initializes an instance of the <see cref="StorageService" /> class.
        /// </summary>
        /// <param name="keyVaultEndpoint">The Azure Key Vault endpoint address.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
        public async Task InitializeAsync(string keyVaultEndpoint)
        {
            IVaultService vaultSerivce;

            try
            {
                vaultSerivce = new KeyVaultService(keyVaultEndpoint);

                if (storageAccount == null)
                {
                    if (!CloudStorageAccount.TryParse(
                        await vaultSerivce.GetSecretAsync(StorageAccountConnectionStringName).ConfigureAwait(false),
                        out storageAccount))
                    {
                        throw new Exception("Unable to intialize the storage account. Please check the connection string setting.");
                    }
                }
            }
            finally
            {
                vaultSerivce = null;
            }
        }

        /// <summary>
        /// Writes the specified entity to an Azure Storage queue.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to be written.</typeparam>
        /// <param name="queueName">Name of the queue where the entity will be written.</param>
        /// <param name="entity">Entity that will be written to the queue.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
        public async Task WriteToQueueAsync<TEntity>(string queueName, TEntity entity)
        {
            CloudQueue queue;
            CloudQueueClient queueClient;

            try
            {
                queueClient = storageAccount.CreateCloudQueueClient();
                queue = queueClient.GetQueueReference(queueName);

                await queue.CreateIfNotExistsAsync().ConfigureAwait(false);

                await queue.AddMessageAsync(
                    new CloudQueueMessage(JsonConvert.SerializeObject(entity))).ConfigureAwait(false);
            }
            finally
            {
                queueClient = null;
            }
        }
    }
}