// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Converters;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.MicrosoftGraph;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using Models;
    using Records;

    public static class Security
    {
        [FunctionName("controlprofilesync")]
        public static async Task ControlProfileSyncAsync(
            [QueueTrigger(
                "controlprofilesync",
                Connection = "StorageConnectionString")]CustomerRecord input,
            [SecureScoreControlProfile(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                ApplicationSecretName = "{AppEndpoint.ApplicationSecretName}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                TenantId = "{Id}")]List<SecureScoreControlProfile> profiles,
            [CosmosDB(
                databaseName: "smartoffice",
                collectionName: "customers",
                ConnectionStringSetting = "CosmosDbConnectionString",
                CreateIfNotExists = true)]IAsyncCollector<CustomerEntry> items)
        {
            CustomerEntry entry = ResourceConverter.Convert<CustomerRecord, CustomerEntry>(input);

            entry.SecureScoreControlProfiles = profiles;

            await items.AddAsync(entry).ConfigureAwait(false);
        }

        [FunctionName("SecuritySync")]
        public static void SecuritySync(
            [QueueTrigger(
                "securitysync",
                Connection = "StorageConnectionString")]CustomerRecord input,
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
                databaseName: "smartoffice",
                collectionName: "securescores",
                ConnectionStringSetting = "CosmosDbConnectionString",
                CreateIfNotExists = true)]IAsyncCollector<SecureScore> secureScores,
            [CosmosDB(
                databaseName: "smartoffice",
                collectionName: "securityalerts",
                ConnectionStringSetting = "CosmosDbConnectionString",
                CreateIfNotExists = true)]IAsyncCollector<Alert> securityAlerts,
            ILogger log)
        {
            alerts.ForEach(async (alert) =>
            {
                await securityAlerts.AddAsync(alert).ConfigureAwait(false);
            });

            scores.ForEach(async (score) =>
            {
                await secureScores.AddAsync(score).ConfigureAwait(false);
            });
        }
    }
}