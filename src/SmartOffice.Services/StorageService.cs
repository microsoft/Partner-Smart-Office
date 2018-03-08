// -----------------------------------------------------------------------
// <copyright file="StorageService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using WindowsAzure.Storage;
    using WindowsAzure.Storage.Queue;

    public class StorageService : IStorageService
    {
        private CloudStorageAccount storageAccount;

        private IVaultSerivce vaultSerivce;

        /// <summary>
        /// Singleton instance of the <see cref="StorageService" /> class.
        /// </summary>
        private static Lazy<StorageService> instance = new Lazy<StorageService>(() => new StorageService());

        public StorageService()
        {
        }

        /// <summary>
        /// Gets an instance of the <see cref="StorageService" /> class.
        /// </summary>
        public static StorageService Instance => instance.Value;

        public async Task InitializeAsync(string keyVaultEndpoint, string connectionString)
        {
            vaultSerivce = new KeyVaultService(keyVaultEndpoint);

            CloudStorageAccount.TryParse(
                await vaultSerivce.GetSecretAsync(connectionString).ConfigureAwait(false),
                out storageAccount);
        }

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