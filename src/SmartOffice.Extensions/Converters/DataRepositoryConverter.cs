// -----------------------------------------------------------------------
// <copyright file="DataRepositoryConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions.Converters
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Bindings;
    using Data;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Models;
    using Models.Graph;
    using Models.PartnerCenter.AuditRecords;
    using Services.KeyVault;

    public class DataRepositoryConverter : IAsyncConverter<DataRepositoryAttribute, object>
    {
        /// <summary>
        /// Name of the audit records collection.
        /// </summary>
        private const string AuditRecordsCollectionId = "AuditRecords";

        /// <summary>
        /// Name of the secret that contains the Comsos DB access key.
        /// </summary>
        private const string CosmsosDbAccessKey = "CosmosDbAccessKey";

        /// <summary>
        /// Identifier for the customers collection.
        /// </summary>
        private const string CustomersCollectionId = "Customers";

        /// <summary>
        /// Identifier for the Azure Cosmos DB database.
        /// </summary>
        private const string DatabaseId = "SmartOffice";

        /// <summary>
        /// Identifier for the environments collection.
        /// </summary>
        private const string EnvironmentsCollectionId = "Environments";

        /// <summary>
        /// Identifier for the security alerts collection.
        /// </summary>
        private const string SecurityAlertsCollectionId = "SecurityAlerts";

        /// <summary>
        /// Identifier for the secure score collection.
        /// </summary>
        private const string SecureScoreCollectionId = "SecureScore";

        /// <summary>
        /// Identifier for the secure score controls collection.
        /// </summary>
        private const string SecureScoreControlsCollectionId = "SecureScoreControls";

        /// <summary>
        /// Identifier for the subscriptions collection.
        /// </summary>
        private const string SubscriptionsCollectionId = "Subscriptions";

        /// <summary>
        /// Identifier for the utilization collection.
        /// </summary>
        private const string UtilizationCollectId = "Utilization";

        /// <summary>
        /// Collection of initialized data repositories.
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> repos = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Used to help ensure that data repositories are initialized in a thread safe manner.
        /// </summary>
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private readonly ILogger log;

        private readonly IVaultService vault;

        private readonly SmartOfficeOptions options;

        public DataRepositoryConverter(ILoggerFactory loggerFactory, IOptions<SmartOfficeOptions> options, IVaultService vault)
        {
            log = loggerFactory?.CreateLogger("Host.Bindings.DataRepositoryConverter");
            this.options = options.Value;
            this.vault = vault;
        }

        public async Task<object> ConvertAsync(DataRepositoryAttribute input, CancellationToken cancellationToken)
        {
            if (input.DataType == typeof(Alert))
            {
                return await GetRepoAsync<Alert>(SecurityAlertsCollectionId).ConfigureAwait(false);
            }
            else if (input.DataType == typeof(AuditRecord))
            {
                return await GetRepoAsync<AuditRecord>(
                    AuditRecordsCollectionId,
                    "/PartnerId").ConfigureAwait(false);
            }
            else if (input.DataType == typeof(ControlListEntry))
            {
                return await GetRepoAsync<ControlListEntry>(
                    SecureScoreControlsCollectionId).ConfigureAwait(false);
            }
            else if (input.DataType == typeof(CustomerDetail))
            {
                return await GetRepoAsync<CustomerDetail>(
                    CustomersCollectionId).ConfigureAwait(false);
            }
            else if (input.DataType == typeof(EnvironmentDetail))
            {
                return await GetRepoAsync<EnvironmentDetail>(
                    EnvironmentsCollectionId).ConfigureAwait(false);
            }
            else if (input.DataType == typeof(SecureScore))
            {
                return await GetRepoAsync<SecureScore>(
                    SecureScoreCollectionId,
                    "/tenantId").ConfigureAwait(false);
            }
            else if (input.DataType == typeof(SubscriptionDetail))
            {
                return await GetRepoAsync<SubscriptionDetail>(
                    SubscriptionsCollectionId,
                    "/tenantId").ConfigureAwait(false);
            }
            else if (input.DataType == typeof(UtilizationDetail))
            {
                return await GetRepoAsync<UtilizationDetail>(
                    UtilizationCollectId,
                    "/subscriptionId").ConfigureAwait(false);
            }

            throw new Exception($"Invalid data type of {input.DataType} specified.");
        }

        private async Task<IDocumentRepository<TEntity>> GetRepoAsync<TEntity>(
            string collectionId,
            string partitionKey = null) where TEntity : class
        {
            DocumentRepository<TEntity> repo;
            string authKey;

            try
            {
                await Semaphore.WaitAsync().ConfigureAwait(false);

                if (!repos.ContainsKey(collectionId))
                {
                    authKey = await vault.GetSecretAsync(options.KeyVaultEndpoint, CosmsosDbAccessKey).ConfigureAwait(false);

                    repo = new DocumentRepository<TEntity>(
                        options.CosmosDbEndpoint,
                        authKey,
                        DatabaseId,
                        collectionId,
                        partitionKey);

                    await repo.InitializeAsync().ConfigureAwait(false);

                    repos[collectionId] = repo;
                }

                return repos[collectionId] as IDocumentRepository<TEntity>;
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());

                throw;
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}