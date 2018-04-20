// -----------------------------------------------------------------------
// <copyright file="Security.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host;
    using Bindings;
    using ClassMaps;
    using CsvHelper;
    using Data;
    using Models;

    public static class Security
    {
        /// <summary>
        /// Path for the SecureScoreControlsList.csv embedded resource.
        /// </summary>
        private const string ControlListEmbeddedResource = "Microsoft.Partner.SmartOffice.Functions.Assets.SecureScoreControlsList.csv";

        [FunctionName("ImportSecureScoreControls")]
        public static async Task ImportControlsAsync(
            [TimerTrigger("0 0 10 * * *")]TimerInfo timerInfo,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(ControlListEntry),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<ControlListEntry> repository,
            TraceWriter log)
        {
            CsvReader reader = null;
            IEnumerable<ControlListEntry> entries;

            try
            {
                using (StreamReader streamReader = new StreamReader(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(ControlListEmbeddedResource)))
                {
                    reader = new CsvReader(streamReader);
                    reader.Configuration.RegisterClassMap<ControlListEntryClassMap>();

                    entries = reader.GetRecords<ControlListEntry>();

                    await repository.AddOrUpdateAsync(entries).ConfigureAwait(false);
                }
            }
            finally
            {
                entries = null;
                reader?.Dispose();
            }
        }

        [FunctionName("ImportSecurityInfo")]
        public static async Task ProcessAsync(
            [QueueTrigger("customers", Connection = "StorageConnectionString")]Customer customer,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(SecureScore),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<SecureScore> repository,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(Alert),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<Alert> securityAlerts,
            [SecureScore(
                ApplicationId = "ApplicationId",
                CustomerId = "{id}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Period = 1,
                Resource = "https://graph.microsoft.com",
                SecretName = "ApplicationSecret")]List<SecureScore> scores,
            [SecurityAlerts(
                ApplicationId = "ApplicationId",
                CustomerId = "{id}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Resource = "https://graph.microsoft.com",
                SecretName = "ApplicationSecret")]List<Alert> alerts,
            TraceWriter log)
        {
            if (scores?.Count > 0)
            {
                await repository.AddOrUpdateAsync(scores).ConfigureAwait(false);
            }

            if (alerts?.Count > 0)
            {
                await securityAlerts.AddOrUpdateAsync(alerts).ConfigureAwait(false);
            }
        }
    }
}