// -----------------------------------------------------------------------
// <copyright file="SecureScores.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions
{
    using System;
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
    using Microsoft.Partner.SmartOffice.Services;
    using Models;

    public static class SecureScores
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

        [FunctionName("ProcessSecureScore")]
        public static async Task ProcessAsync(
            [QueueTrigger("customers", Connection = "StorageConnectionString")]Customer customer,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(SecureScore),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<SecureScore> repository,
            [SecureScore(
                ApplicationId = "ApplicationId",
                CustomerId = "{id}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Period = 1,
                Resource = "https://graph.microsoft.com",
                SecretName = "ApplicationSecret"
            )]List<SecureScore> secureScore,
            TraceWriter log)
        {
            try
            {
                await repository.AddOrUpdateAsync(secureScore).ConfigureAwait(false);
            }
            catch (ServiceException ex)
            {
                log.Error($"Microsoft Graph returned {ex.Message} with a status code of {ex.HtttpStatusCode}", ex);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex, ex.Source);
            }
        }
    }
}