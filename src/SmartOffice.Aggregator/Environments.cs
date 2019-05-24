// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Models;
    using Records;

    public static class Environments
    {
        [FunctionName("GetEnvironments")]
        public static void GetEnvironments(
            [TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [CosmosDB(
                databaseName: "smartoffice",
                collectionName: "environments",
                ConnectionStringSetting = "ComosDbConnectionString",
                CreateIfNotExists = true,
                SqlQuery = "SELECT * FROM environments")]IEnumerable<EnvironmentEntry> environments,
            [Queue(
                "partnerdeltasync", Connection = "StorageConnectionString")] ICollector<EnvironmentRecord> deltaSyncQueue,
            [Queue(
                "partnerfullasync", Connection = "StorageConnectionString")] ICollector<EnvironmentRecord> fullSyncQueue,
            ILogger log)
        {
            EnvironmentRecord record;
            int days;


            foreach (EnvironmentEntry entry in environments)
            {
                log.LogInformation(entry.FriendlyName);

                // Calculate the number of days that have gone by, since the last successful synchronization.
                days = (entry.LastProcessed == null) ? 30 : (DateTimeOffset.UtcNow - entry.LastProcessed).Value.Days;

                record = new EnvironmentRecord
                {
                    AppEndpoint = entry.AppEndpoint,
                    FriendlyName = entry.FriendlyName,
                    Id = entry.Id,
                    LastProcessed = entry.LastProcessed,
                    PartnerCenterEndpoint = entry.PartnerCenterEndpoint
                };

                if (days >= 30)
                {
                    // Perform a full sync because the environment has not been processed in the past 30 days.
                    fullSyncQueue.Add(record);
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

                    deltaSyncQueue.Add(record);
                }
            }
        }
    }
}