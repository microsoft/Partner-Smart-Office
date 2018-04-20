// -----------------------------------------------------------------------
// <copyright file="SmartOfficeExtensionConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host.Config;
    using Data;
    using Extensions.Logging;
    using Microsoft.Azure.WebJobs.Logging;
    using Models;
    using Services;

    public class SmartOfficeExtensionConfig :
        IAsyncConverter<DataRepositoryAttribute, object>,
        IAsyncConverter<PartnerServiceAttribute, PartnerService>,
        IAsyncConverter<SecureScoreAttribute, List<SecureScore>>,
        IAsyncConverter<SecurityAlertsAttribute, List<Alert>>,
        IAsyncConverter<StorageServiceAttribute, StorageService>,
        IExtensionConfigProvider
    {
        /// <summary>
        /// Identifier for the bulk import stored procedure.
        /// </summary>
        public const string BulkImportStoredProcedureId = "BulkImport";

        /// <summary>
        /// Name of the secret that contains the Comsos DB access key.
        /// </summary>
        private const string CosmsosDbAccessKey = "CosmosDbAccessKey";

        /// <summary>
        /// Name of the Cosmos DB endpoint setting.
        /// </summary>
        private const string CosmosDbEndpoint = "CosmosDbEndpoint";

        /// <summary>
        /// Identifier for the customers collection.
        /// </summary>
        public const string CustomersCollectionId = "Customers";

        /// <summary>
        /// Identifier for the Azure Cosmos DB database.
        /// </summary>
        public const string DatabaseId = "SmartOffice";

        /// <summary>
        /// Name of the Key Vault endpoint setting.
        /// </summary>
        private const string KeyVaultEndpoint = "KeyVaultEndpoint";

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

                throw new Exception($"Invalid data type of {input.DataType} specified.");
            }
            finally
            {
                keyVault = null;
            }
        }

        public async Task<PartnerService> ConvertAsync(PartnerServiceAttribute input, CancellationToken cancellationToken)
        {
            IVaultService vaultService;

            try
            {
                vaultService = new KeyVaultService(input.KeyVaultEndpoint);

                return new PartnerService(new Uri("https://api.partnercenter.microsoft.com"),
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
            catch (ServiceException ex)
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
            catch (ServiceException ex)
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
            context.AddBindingRule<PartnerServiceAttribute>().BindToInput<PartnerService>(this);
            context.AddBindingRule<SecureScoreAttribute>().BindToInput<List<SecureScore>>(this);
            context.AddBindingRule<SecurityAlertsAttribute>().BindToInput<List<Alert>>(this);
            context.AddBindingRule<StorageServiceAttribute>().BindToInput<StorageService>(this);
        }
    }
}