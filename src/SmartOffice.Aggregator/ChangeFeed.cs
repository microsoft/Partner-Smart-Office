// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    using System.Collections.Generic;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    public static class ChangeFeed
    {
        [FunctionName(Constants.ChangeFeedEnvironments)]
        public static void Run(
            [CosmosDBTrigger(
                databaseName: Constants.DatabaseName,
                collectionName: Constants.EnvironmentsCollection,
                ConnectionStringSetting = Constants.CosmosDbConnectionString,
                LeaseCollectionName = Constants.LeasesCollection)]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}