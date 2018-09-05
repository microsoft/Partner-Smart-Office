// -----------------------------------------------------------------------
// <copyright file="DataControls.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host;
    using ClassMaps;
    using CsvHelper;
    using Data;
    using Extensions.Bindings;
    using Models.Graph;

    /// <summary>
    /// Contains the defintion for the Azure functions related to data controls.
    /// </summary>
    public static class DataControls
    {
        /// <summary>
        /// Path for the SecureScoreControlsList.csv embedded resource.
        /// </summary>
        private const string ControlListEmbeddedResource = "Microsoft.Partner.SmartOffice.Functions.Assets.SecureScoreControlsList.csv";

        [FunctionName("ImportDataControls")]
        public static async Task ImportControlsAsync(
            [TimerTrigger("0 0 10 * * *")]TimerInfo timerInfo,
            [DataRepository(
                DataType = typeof(ControlListEntry))]DocumentRepository<ControlListEntry> repository,
            TraceWriter log)
        {
            CsvReader reader = null;
            List<ControlListEntry> entries;

            try
            {
                if (timerInfo.IsPastDue)
                {
                    log.Info("Execution of the function is starting behind schedule.");
                }

                using (StreamReader streamReader = new StreamReader(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(ControlListEmbeddedResource)))
                {
                    reader = new CsvReader(streamReader);
                    reader.Configuration.RegisterClassMap<ControlListEntryClassMap>();

                    entries = reader.GetRecords<ControlListEntry>().ToList();

                    log.Info($"Importing {entries.Count} Office 365 Secure Score controls details...");

                    await repository.AddOrUpdateAsync(entries).ConfigureAwait(false);
                }

                log.Info("Successfully import Office 365 Secure Score controls.");
            }
            finally
            {
                entries = null;
                reader?.Dispose();
            }
        }
    }
}