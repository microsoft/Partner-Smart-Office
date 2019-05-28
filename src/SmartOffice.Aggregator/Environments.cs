// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Models;

    public static class Environments
    {
        [FunctionName(Constants.StartSync)]
        public static async Task GetEnvironmentsAsync(
            [TimerTrigger("0 0 4 * * *")]TimerInfo myTimer,
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: Constants.EnvironmentsCollection,
                ConnectionStringSetting = Constants.CosmosDbConnectionString)]DocumentClient client,
            [Queue(Constants.PartnerDeltaSync, Connection = Constants.StorageConnectionString)]IAsyncCollector<EnvironmentRecord> partnerDeltaSyncQueue,
            [Queue(Constants.PartnerFullSync, Connection = Constants.StorageConnectionString)]IAsyncCollector<EnvironmentRecord> partnerFullSyncQueue,
            ILogger log)
        {
            EnvironmentRecord record;
            int days;

            List<EnvironmentEntry> environments = await DocumentRepository.QueryAsync<EnvironmentEntry>(
                client,
                Constants.DatabaseName,
                Constants.EnvironmentsCollection,
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

            environments.ForEach(async (entry) =>
            {
                log.LogInformation(entry.EnvironmentName);

                // Calculate the number of days that have gone by, since the last successful synchronization.
                days = (entry.LastProcessed == null) ? 30 : (DateTimeOffset.UtcNow - entry.LastProcessed).Value.Days;

                record = new EnvironmentRecord
                {
                    AppEndpoint = entry.AppEndpoint,
                    EnvironmentId = entry.EnvironmentId,
                    EnvironmentName = entry.EnvironmentName,
                    Id = entry.Id,
                    LastProcessed = entry.LastProcessed,
                    PartnerCenterEndpoint = entry.PartnerCenterEndpoint
                };

                if (days >= 30)
                {
                    // Perform a full sync because the environment has not been processed in the past 30 days.
                    await partnerFullSyncQueue.AddAsync(record).ConfigureAwait(false);
                }
                else
                {
                    /* 
                     * Perform a delta sync because the environment has recently been processed. This type of sync
                     * will utilize the audit records available from Partner Center to determine what changes have 
                     * been made since the last successful sync. This will result in a more rapid execution of the 
                     * the processs to update the environment and obtain information from Microsoft Graph.
                     */

                    record.AuditEndDate = DateTime.UtcNow.ToString();
                    record.AuditStartDate = DateTime.UtcNow.AddDays(-days).ToString();

                    await partnerDeltaSyncQueue.AddAsync(record).ConfigureAwait(false);
                }
            });
        }
    }
}