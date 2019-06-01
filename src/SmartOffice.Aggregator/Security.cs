// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.AzureMonitor;
    using Microsoft.Azure.WebJobs.Extensions.MicrosoftGraph;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using Models;

    /// <summary>
    /// Defines the Azure Functions used to aggregate security information.
    /// </summary>
    public static class Security
    {
        [FunctionName(Constants.ControlProfileSync)]
        public static async Task ControlProfileSyncAsync(
            [QueueTrigger(Constants.ControlProfileSync, Connection = Constants.StorageConnectionString)]CustomerRecord input,
            [SecureScoreControlProfile(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                ApplicationSecretName = "{AppEndpoint.ApplicationSecretName}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                TenantId = "{Id}")]List<SecureScoreControlProfile> profiles,
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: "securityevents",
                ConnectionStringSetting = Constants.CosmosDbConnectionString,
                CreateIfNotExists = true,
                PartitionKey = "/customerId")]IAsyncCollector<SecurityDataEntry<List<SecureScoreControlProfile>>> output,
            ILogger log)
        {
            log.LogInformation($"Processing {profiles.Count} Secure Score control profiles for {input.Name}");

            await output.AddAsync(new SecurityDataEntry<List<SecureScoreControlProfile>>
            {
                CustomerId = input.Id,
                CustomerName = input.Name,
                Entry = profiles,
                EnvironmentId = input.EnvironmentId,
                EnvironmentName = input.EnvironmentName,
                Id = $"{input.Id}-controlprofile"
            }).ConfigureAwait(false);
        }

        [FunctionName(Constants.SecurtityEventSync)]
        public static async Task SecurityEventSyncAsync(
            [QueueTrigger(Constants.SecurtityEventSync, Connection = Constants.StorageConnectionString)]CustomerRecord input,
            [DirectoryAudit(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                ApplicationSecretName = "{AppEndpoint.ApplicationSecretName}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                TenantId = "{Id}")]List<DirectoryAudit> logs,
            [SecureScore(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                ApplicationSecretName = "{AppEndpoint.ApplicationSecretName}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                Period = "{SecureScorePeriod}",
                TenantId = "{Id}")]List<SecureScore> scores,
            [SecurityAlert(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                ApplicationSecretName = "{AppEndpoint.ApplicationSecretName}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                TenantId = "{Id}")]List<Alert> alerts,
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: "securityevents",
                ConnectionStringSetting = Constants.CosmosDbConnectionString,
                CreateIfNotExists = true,
                PartitionKey = "/customerId")]IAsyncCollector<BaseDataEntry> output,
            [DataCollector(
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                WorkspaceId = "{WorkspaceId}",
                WorkspaceKeyName = "{WorkspaceKeyName}")]IAsyncCollector<LogData> datacCollector,
            ILogger log)
        {
            log.LogInformation($"Processing {logs.Count} directory audit logs for {input.Name}");

            foreach (DirectoryAudit auditLog in logs)
            {
                await output.AddAsync(new SecurityDataEntry<DirectoryAudit>
                {
                    CustomerId = input.Id,
                    CustomerName = input.Name,
                    Entry = auditLog,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = auditLog.Id
                }).ConfigureAwait(false);
            }

            await datacCollector.AddAsync(new LogData
            {
                Data = logs,
                LogType = nameof(DirectoryAudit),
                TimeStampField = "ActivityDateTime"
            });

            log.LogInformation($"Processing {alerts.Count} alerts for {input.Name}");

            foreach (Alert alert in alerts)
            {
                await output.AddAsync(new SecurityDataEntry<Alert>
                {
                    CustomerId = input.Id,
                    CustomerName = input.Name,
                    Entry = alert,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = alert.Id
                }).ConfigureAwait(false);
            }

            await datacCollector.AddAsync(new LogData
            {
                Data = alerts,
                LogType = nameof(Alert),
                TimeStampField = "CreatedDateTime"
            });

            log.LogInformation($"Processing {scores.Count} Secure Score entries for {input.Name}");

            foreach (SecureScore score in scores)
            {
                await output.AddAsync(new SecurityDataEntry<SecureScore>
                {
                    CustomerId = input.Id,
                    CustomerName = input.Name,
                    Entry = score,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = score.Id
                }).ConfigureAwait(false);
            }

            await datacCollector.AddAsync(new LogData
            {
                Data = scores,
                LogType = nameof(SecureScore),
                TimeStampField = "CreatedDateTime"
            });
        }
    }
}