// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
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
                TenantId = "{Id}")]List<SecureScoreControlProfile> controlProfiles,
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: "securityevents",
                ConnectionStringSetting = Constants.CosmosDbConnectionString,
                CreateIfNotExists = true,
                PartitionKey = "/customerId")]IAsyncCollector<SecurityDataEntry<List<SecureScoreControlProfile>>> profiles)
        {
            await profiles.AddAsync(new SecurityDataEntry<List<SecureScoreControlProfile>>
            {
                CustomerId = input.Id,
                CustomerName = input.Name,
                Entry = controlProfiles,
                EnvironmentId = input.EnvironmentId,
                EnvironmentName = input.EnvironmentName,
                Id = $"{input.Id}-controlprofile"
            }).ConfigureAwait(false);
        }

        [FunctionName(Constants.SecurtityEventSync)]
        public static void SecurityEventSync(
            [QueueTrigger(Constants.SecurtityEventSync, Connection = Constants.StorageConnectionString)]CustomerRecord input,
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
                PartitionKey = "/customerId")]IAsyncCollector<SecurityDataEntry<SecureScore>> secureScores,
            [CosmosDB(
                databaseName: Constants.DatabaseName,
                collectionName: "securityevents",
                ConnectionStringSetting = Constants.CosmosDbConnectionString,
                CreateIfNotExists = true,
                PartitionKey = "/customerId")]IAsyncCollector<SecurityDataEntry<Alert>> securityAlerts,
            ILogger log)
        {
            log.LogInformation($"Processing {alerts.Count} alerts for {input.Name}");

            alerts.ForEach(async (alert) =>
            {
                await securityAlerts.AddAsync(new SecurityDataEntry<Alert>
                {
                    CustomerId = input.Id,
                    CustomerName = input.Name,
                    Entry = alert,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = alert.Id
                }).ConfigureAwait(false);
            });

            log.LogInformation($"Processing {scores.Count} Secure Score entries for {input.Name}");

            scores.ForEach(async (score) =>
            {
                await secureScores.AddAsync(new SecurityDataEntry<SecureScore>
                {
                    CustomerId = input.Id,
                    CustomerName = input.Name,
                    Entry = score,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = score.Id
                }).ConfigureAwait(false);
            });
        }
    }
}