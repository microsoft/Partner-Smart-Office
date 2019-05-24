// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    using System.Collections.Generic;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.MicrosoftGraph;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using Records;

    public static class Security
    {
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
            [CosmosDB(
                databaseName: "securescores",
                collectionName: "customers",
                ConnectionStringSetting = "CosmosDbConnectionString",
                CreateIfNotExists = true)]IAsyncCollector<SecureScore> items,
            ILogger log)
        {

            scores.ForEach(async (score) =>
            {
                await items.AddAsync(score).ConfigureAwait(false);
            });
        }
    }
}