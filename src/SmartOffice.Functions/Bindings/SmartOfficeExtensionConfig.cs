// -----------------------------------------------------------------------
// <copyright file="SmartOfficeExtensionConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Bindings
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host.Config;
    using Azure.WebJobs.Logging;
    using Data;
    using Extensions.Logging;
    using Models;
    using Models.Graph;
    using Models.PartnerCenter.AuditRecords;
    using Services;
    using Services.PartnerCenter;

    public class SmartOfficeExtensionConfig :
        IAsyncConverter<DataRepositoryAttribute, object>,
        IAsyncConverter<PartnerServiceAttribute, PartnerServiceClient>,
        IAsyncConverter<SecureScoreAttribute, List<SecureScore>>,
        IAsyncConverter<SecurityAlertsAttribute, List<Alert>>,
        IAsyncConverter<StorageServiceAttribute, StorageService>,
        IExtensionConfigProvider
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
        private static readonly ConcurrentDictionary<string, object> Repos = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// Used to help ensure that data repositories are initialized in a thread safe manner.
        /// </summary>
        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Provides the ability to capture log information.
        /// </summary>
        private ILogger log;

        public async Task<object> ConvertAsync(DataRepositoryAttribute input, CancellationToken cancellationToken)
        {
            if (input.DataType == typeof(Alert))
            {
                return await GetRepoAsync<Alert>(
                    SecurityAlertsCollectionId,
                    input.CosmosDbEndpoint,
                    input.KeyVaultEndpoint).ConfigureAwait(false);
            }
            else if (input.DataType == typeof(AuditRecord))
            {
                return await GetRepoAsync<AuditRecord>(
                    AuditRecordsCollectionId,
                    input.CosmosDbEndpoint,
                    input.KeyVaultEndpoint,
                    "/PartnerId").ConfigureAwait(false);
            }
            else if (input.DataType == typeof(ControlListEntry))
            {
                return await GetRepoAsync<ControlListEntry>(
                    SecureScoreControlsCollectionId,
                    input.CosmosDbEndpoint,
                    input.KeyVaultEndpoint).ConfigureAwait(false);
            }
            else if (input.DataType == typeof(CustomerDetail))
            {
                return await GetRepoAsync<CustomerDetail>(
                    CustomersCollectionId,
                    input.CosmosDbEndpoint,
                    input.KeyVaultEndpoint).ConfigureAwait(false);
            }
            else if (input.DataType == typeof(EnvironmentDetail))
            {
                return await GetRepoAsync<EnvironmentDetail>(
                    EnvironmentsCollectionId,
                    input.CosmosDbEndpoint,
                    input.KeyVaultEndpoint).ConfigureAwait(false);
            }
            else if (input.DataType == typeof(SecureScore))
            {
                return await GetRepoAsync<SecureScore>(
                    SecureScoreCollectionId,
                    input.CosmosDbEndpoint,
                    input.KeyVaultEndpoint,
                    "/tenantId").ConfigureAwait(false);
            }
            else if (input.DataType == typeof(SubscriptionDetail))
            {
                return await GetRepoAsync<SubscriptionDetail>(
                    SubscriptionsCollectionId,
                    input.CosmosDbEndpoint,
                    input.KeyVaultEndpoint,
                    "/tenantId").ConfigureAwait(false);
            }
            else if (input.DataType == typeof(UtilizationDetail))
            {
                return await GetRepoAsync<UtilizationDetail>(
                    UtilizationCollectId,
                    input.CosmosDbEndpoint,
                    input.KeyVaultEndpoint,
                    "/subscriptionId").ConfigureAwait(false);
            }

            throw new Exception($"Invalid data type of {input.DataType} specified.");
        }

        public async Task<PartnerServiceClient> ConvertAsync(PartnerServiceAttribute input, CancellationToken cancellationToken)
        {
            IVaultService vaultService;

            try
            {
                vaultService = new KeyVaultService(input.KeyVaultEndpoint);

                return new PartnerServiceClient(
                    new Uri(input.Endpoint),
                    new ServiceCredentials(
                        input.ApplicationId,
                        await vaultService.GetSecretAsync(input.SecretName).ConfigureAwait(false),
                        input.Resource,
                        input.ApplicationTenantId),
                    new PartnerServiceMessageHandler());
            }
            finally
            {
                vaultService = null;
            }
        }

        public async Task<List<SecureScore>> ConvertAsync(SecureScoreAttribute input, CancellationToken cancellationToken)
        {
            GraphService graphService;
            IVaultService vaultService;
            List<SecureScore> secureScore;

            try
            {
                vaultService = new KeyVaultService(input.KeyVaultEndpoint);

                graphService = new GraphService(
                    new Uri(input.Resource),
                    new ServiceCredentials(
                        input.ApplicationId,
                        await vaultService.GetSecretAsync(input.SecretName).ConfigureAwait(false),
                        input.Resource,
                        input.CustomerId));

                secureScore = await graphService.GetSecureScoreAsync(int.Parse(input.Period, CultureInfo.CurrentCulture), cancellationToken).ConfigureAwait(false);

                return secureScore;
            }
            catch (ServiceClientException ex)
            {
                log.LogError(ex, $"Encountered {ex} when processing {input.CustomerId}");
                return null;
            }
            finally
            {
                graphService = null;
                vaultService = null;
            }
        }

        public async Task<List<Alert>> ConvertAsync(SecurityAlertsAttribute input, CancellationToken cancellationToken)
        {
            GraphService graphService;
            IVaultService vaultService;
            List<Alert> alerts;

            try
            {
                vaultService = new KeyVaultService(input.KeyVaultEndpoint);

                graphService = new GraphService(new Uri(input.Resource),
                    new ServiceCredentials(
                        input.ApplicationId,
                        await vaultService.GetSecretAsync(input.SecretName).ConfigureAwait(false),
                        input.Resource,
                        input.CustomerId));

                alerts = await graphService.GetAlertsAsync(cancellationToken).ConfigureAwait(false);

                return alerts;
            }
            catch (ServiceClientException ex)
            {
                log.LogError(ex, $"Encountered {ex.Message} when processing {input.CustomerId}");
                return null;
            }
            finally
            {
                graphService = null;
                vaultService = null;
            }
        }

        public async Task<StorageService> ConvertAsync(StorageServiceAttribute input, CancellationToken cancellationToken)
        {
            await StorageService.Instance.InitializeAsync(
                input.KeyVaultEndpoint,
                input.ConnectionStringName).ConfigureAwait(false);

            return StorageService.Instance;
        }

        /// <summary>
        /// Initialize the binding extension
        /// </summary>
        /// <param name="context">Context for the extension</param>
        public void Initialize(ExtensionConfigContext context)
        {
            log = context.Config.LoggerFactory.CreateLogger(LogCategories.CreateFunctionCategory("SmartOffice"));

            context.AddBindingRule<DataRepositoryAttribute>().BindToInput<object>(this);
            context.AddBindingRule<PartnerServiceAttribute>().BindToInput<PartnerServiceClient>(this);
            context.AddBindingRule<SecureScoreAttribute>().BindToInput<List<SecureScore>>(this);
            context.AddBindingRule<SecurityAlertsAttribute>().BindToInput<List<Alert>>(this);
            context.AddBindingRule<StorageServiceAttribute>().BindToInput<StorageService>(this);
        }

        private static async Task<IDocumentRepository<TEntity>> GetRepoAsync<TEntity>(
            string collectionId,
            string cosmosDbEndpoint,
            string keyVaultEndpoint,
            string partitionKey = null) where TEntity : class
        {
            DocumentRepository<TEntity> repo;
            KeyVaultService keyVault;
            string authKey;

            try
            {
                await Semaphore.WaitAsync().ConfigureAwait(false);

                if (!Repos.ContainsKey(collectionId))
                {
                    keyVault = new KeyVaultService(keyVaultEndpoint);
                    authKey = await keyVault.GetSecretAsync(CosmsosDbAccessKey).ConfigureAwait(false);

                    repo = new DocumentRepository<TEntity>(
                        cosmosDbEndpoint,
                        authKey,
                        DatabaseId,
                        collectionId,
                        partitionKey);

                    await repo.InitializeAsync().ConfigureAwait(false);

                    Repos[collectionId] = repo;
                }

                return Repos[collectionId] as IDocumentRepository<TEntity>;
            }
            finally
            {
                keyVault = null;

                Semaphore.Release();
            }
        }
    }
}