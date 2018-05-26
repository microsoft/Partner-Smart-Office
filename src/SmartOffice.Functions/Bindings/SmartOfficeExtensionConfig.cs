// -----------------------------------------------------------------------
// <copyright file="SmartOfficeExtensionConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host.Config;
    using Azure.WebJobs.Logging;
    using Data;
    using Extensions.Logging;
    using Models;
    using Models.Graph;
    using Models.PartnerCenter;
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
        /// Provides the ability to capture log information.
        /// </summary>
        private ILogger log;

        public async Task<object> ConvertAsync(DataRepositoryAttribute input, CancellationToken cancellationToken)
        {
            KeyVaultService keyVault;
            string authKey;

            try
            {
                keyVault = new KeyVaultService(input.KeyVaultEndpoint);

                authKey = await keyVault.GetSecretAsync(CosmsosDbAccessKey).ConfigureAwait(false);

                if (input.DataType == typeof(Alert))
                {
                    DocumentRepository<Alert> securityAlerts = new DocumentRepository<Alert>(
                        input.CosmosDbEndpoint,
                        authKey,
                        DatabaseId,
                        SecurityAlertsCollectionId);

                    await securityAlerts.InitializeAsync().ConfigureAwait(false);

                    return securityAlerts;
                }
                else if (input.DataType == typeof(AuditRecord))
                {
                    DocumentRepository<AuditRecord> auditRecords = new DocumentRepository<AuditRecord>(
                        input.CosmosDbEndpoint,
                        authKey,
                        DatabaseId,
                        AuditRecordsCollectionId);

                    await auditRecords.InitializeAsync().ConfigureAwait(false);

                    return auditRecords;
                }
                else if (input.DataType == typeof(ControlListEntry))
                {
                    DocumentRepository<ControlListEntry> controls = new DocumentRepository<ControlListEntry>(
                        input.CosmosDbEndpoint,
                        authKey,
                        DatabaseId,
                        SecureScoreControlsCollectionId);

                    await controls.InitializeAsync().ConfigureAwait(false);

                    return controls;
                }
                else if (input.DataType == typeof(Customer))
                {
                    DocumentRepository<Customer> customers = new DocumentRepository<Customer>(
                        input.CosmosDbEndpoint,
                        authKey,
                        DatabaseId,
                        CustomersCollectionId);

                    await customers.InitializeAsync().ConfigureAwait(false);

                    return customers;
                }
                else if (input.DataType == typeof(EnvironmentDetail))
                {
                    DocumentRepository<EnvironmentDetail> environments = new DocumentRepository<EnvironmentDetail>(
                        input.CosmosDbEndpoint,
                        authKey,
                        DatabaseId,
                        EnvironmentsCollectionId);

                    await environments.InitializeAsync().ConfigureAwait(false);

                    return environments;
                }
                else if (input.DataType == typeof(SecureScore))
                {
                    DocumentRepository<SecureScore> score = new DocumentRepository<SecureScore>(
                        input.CosmosDbEndpoint,
                        authKey,
                        DatabaseId,
                        SecureScoreCollectionId);

                    await score.InitializeAsync().ConfigureAwait(false);

                    return score;
                }
                else if (input.DataType == typeof(Subscription))
                {
                    DocumentRepository<Subscription> subscriptions = new DocumentRepository<Subscription>(
                        input.CosmosDbEndpoint,
                        authKey,
                        DatabaseId,
                        SubscriptionsCollectionId);

                    await subscriptions.InitializeAsync().ConfigureAwait(false);

                    return subscriptions;
                }

                throw new Exception($"Invalid data type of {input.DataType} specified.");
            }
            finally
            {
                keyVault = null;
            }
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
                        input.ApplicationTenantId));
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

                secureScore = await graphService.GetSecureScoreAsync(input.Period).ConfigureAwait(false);

                return secureScore;
            }
            catch (ServiceClientException ex)
            {
                log.LogError(ex, $"Encountered {ex.ToString()} when processing {input.CustomerId}");
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

                alerts = await graphService.GetAlertsAsync().ConfigureAwait(false);

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
    }
}