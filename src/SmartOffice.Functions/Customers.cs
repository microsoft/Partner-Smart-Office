// -----------------------------------------------------------------------
// <copyright file="Customers.cs" company="Microsoft">
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
    using Data;
    using Models;
    using Models.Graph;

    /// <summary>
    /// Contains the defintion for the Azure functions related to environments.
    /// </summary>
    public static class Customers
    {
        [FunctionName("ProcessCustomer")]
        public static async Task ProcessCustomerAsync(
            [QueueTrigger("customers", Connection = "StorageConnectionString")]CustomerDetail customer,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(SecureScore),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<SecureScore> repository,
            [DataRepository(
                CosmosDbEndpoint = "CosmosDbEndpoint",
                DataType = typeof(Alert),
                KeyVaultEndpoint = "KeyVaultEndpoint")]IDocumentRepository<Alert> securityAlerts,
            [SecureScore(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                CustomerId = "{Id}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Period = 1,
                Resource = "https://graph.microsoft.com",
                SecretName = "{AppEndpoint.ApplicationSecretId}")]List<SecureScore> scores,
            [SecurityAlerts(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                CustomerId = "{Id}",
                KeyVaultEndpoint = "KeyVaultEndpoint",
                Resource = "https://graph.microsoft.com",
                SecretName = "{AppEndpoint.ApplicationSecretId}")]List<Alert> alerts,
            TraceWriter log)
        {
            if (scores?.Count > 0)
            {
                await repository.AddOrUpdateAsync(scores).ConfigureAwait(false);
            }

            if (alerts?.Count > 0)
            {
                await securityAlerts.AddOrUpdateAsync(alerts).ConfigureAwait(false);
            }
        }
    }
}