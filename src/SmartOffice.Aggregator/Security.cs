// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.Azure.Documents.Client;
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
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: Constants.EnvironmentsCollection,
                ConnectionStringSetting = Constants.CosmosDbConnectionString)]DocumentClient client,
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
            [DataCollector(
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                WorkspaceId = "{WorkspaceId}",
                WorkspaceKeyName = "{WorkspaceKeyName}")]IAsyncCollector<LogData> datacCollector,
            ILogger log)
        {
            log.LogInformation($"Processing {logs.Count} directory audit logs for {input.Name}");

            await DocumentRepository.AddOrUpdateAsync(
                client,
                Constants.DatabaseName,
                Constants.SecurityEventsCollection,
                input.Id,
                logs.Select(item => new SecurityDataEntry<DirectoryAudit>
                {
                    CustomerId = input.Id,
                    CustomerName = input.Name,
                    Entry = item,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = item.Id
                })).ConfigureAwait(false);

            await datacCollector.AddAsync(new LogData
            {
                Data = logs,
                LogType = nameof(DirectoryAudit),
                TimeStampField = "ActivityDateTime"
            }).ConfigureAwait(false);

            log.LogInformation($"Processing {alerts.Count} alerts for {input.Name}");

            await DocumentRepository.AddOrUpdateAsync(
                client,
                Constants.DatabaseName,
                Constants.SecurityEventsCollection,
                input.Id,
                alerts.Select(item => new SecurityDataEntry<Alert>
                {
                    CustomerId = input.Id,
                    CustomerName = input.Name,
                    Entry = item,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = item.Id
                })).ConfigureAwait(false);

            await datacCollector.AddAsync(new LogData
            {
                Data = alerts,
                LogType = nameof(Alert),
                TimeStampField = "CreatedDateTime"
            }).ConfigureAwait(false);

            log.LogInformation($"Processing {scores.Count} Secure Score entries for {input.Name}");

            await DocumentRepository.AddOrUpdateAsync(
                client,
                Constants.DatabaseName,
                Constants.SecurityEventsCollection,
                input.Id,
                scores.Select(item => new SecurityDataEntry<SecureScore>
                {
                    CustomerId = input.Id,
                    CustomerName = input.Name,
                    Entry = item,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = item.Id
                })).ConfigureAwait(false);

            await datacCollector.AddAsync(new LogData
            {
                Data = scores,
                LogType = nameof(SecureScore),
                TimeStampField = "CreatedDateTime"
            }).ConfigureAwait(false);
        }
    }
}