// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Models;
    using Models.Converters;
    using Telemetry;

    /// <summary>
    /// Defines the Azure Functions used process environment information.
    /// </summary>
    public class Environments
    {
        /// <summary>
        /// Provides the ability to manage a document repository.
        /// </summary>
        private readonly IDocumentRepository repository;

        /// <summary>
        /// Provides the ability to capture telemetry.
        /// </summary>
        private readonly ITelemetryProvider telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="Environments" /> class.
        /// </summary>
        /// <param name="repository">Provides the ability to manage a document repository.</param>
        /// <param name="telemetry">Provides the ability to capture telemetry.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="repository"/> is null
        /// or
        /// <paramref name="telemetry"/> is null
        /// </exception>
        public Environments(IDocumentRepository repository, ITelemetryProvider telemetry)
        {
            repository.AssertNotNull(nameof(repository));
            telemetry.AssertNotNull(nameof(telemetry));

            this.repository = repository;
            this.telemetry = telemetry;
        }

        [FunctionName(OperationConstants.StartSync)]
        public async Task StartSyncAsync(
            [TimerTrigger("0 0 4 * * *")]TimerInfo timer,
            [Queue(OperationConstants.PartnerDeltaSync, Connection = OperationConstants.StorageConnectionString)]IAsyncCollector<EnvironmentRecord> partnerDeltaSyncQueue,
            [Queue(OperationConstants.PartnerFullSync, Connection = OperationConstants.StorageConnectionString)]IAsyncCollector<EnvironmentRecord> partnerFullSyncQueue,
            ILogger log)
        {
            EnvironmentRecord record;
            DateTime executionTime = DateTime.UtcNow;

            if (timer.IsPastDue)
            {
                log.LogInformation("Execution of this function has started late.");
            }

            List<DataEntry<EnvironmentEntry>> environments = await repository.QueryAsync<DataEntry<EnvironmentEntry>>(
                OperationConstants.DatabaseId,
                OperationConstants.EnvironmentsCollectionId,
                "/environmentId",
                new SqlQuerySpec
                {
                    Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@entryType", nameof(EnvironmentEntry)),
                    },
                    QueryText = "SELECT * FROM c WHERE c.entryType = @entryType",
                },
                true).ConfigureAwait(false);

            foreach (DataEntry<EnvironmentEntry> entry in environments)
            {
                record = ResourceConverter.Convert<DataEntry<EnvironmentEntry>, EnvironmentRecord>(entry);

                record.AuditEndDate = DateTime.UtcNow.ToString();
                // TODO - Once the logic to track when the environment was last processed has been added 
                // the following statement needs to be updated to reflect the appropriate start date.
                record.AuditStartDate = DateTime.UtcNow.AddDays(-30).ToString();

                // TODO - Once the logic to track when the environment was last process has been added
                // there should be conditional statement to determine if a delta or full sync is required.
                await partnerFullSyncQueue.AddAsync(record).ConfigureAwait(false);
            }

            Dictionary<string, double> eventMetrics = new Dictionary<string, double>
            {
                { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds },
                { "NumberOfEnvironments", environments.Count }
            };

            Dictionary<string, string> eventProperties = new Dictionary<string, string>
            {
            };

            telemetry.TrackEvent(nameof(StartSyncAsync), eventProperties, eventMetrics);
        }
    }
}