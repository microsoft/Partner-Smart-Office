// -----------------------------------------------------------------------
// <copyright file="ProcessSecureScore.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host;
    using Bindings;
    using Models;
    using Services;

    public static class ProcessSecureScore
    {
        [FunctionName("ProcessSecureScore")]
        public static async Task RunAsync(
            [QueueTrigger("customers", Connection = "StorageConnectionString")]Customer customer,
            [SecureScore(
                ApplicationId = "ApplicationId",
                CustomerId = "{id}",
                Period = 1,
                Resource = "https://graph.microsoft.com",
                SecretName = "ApplicationSecret"
            )]List<SecureScore> secureScore,
            TraceWriter log)
        {

            await CosmosDbService.Instance.ExecuteStoredProcedureAsync(
                DataConstants.DatabaseId,
                DataConstants.SecureScoreCollectionId,
                DataConstants.BulkImportStoredProcedureId,
                secureScore).ConfigureAwait(false);
        }
    }
}