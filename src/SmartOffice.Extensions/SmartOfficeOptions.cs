// -----------------------------------------------------------------------
// <copyright file="SmartOfficeWebJobsBuilderExtensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions
{
    using Azure.WebJobs;

    public class SmartOfficeOptions
    {
        public string CosmosDbEndpoint { get; set; }

        public string KeyVaultEndpoint { get; set; }

        public void SetAppSettings(INameResolver nameResolver)
        {
            CosmosDbEndpoint = nameResolver.Resolve("CosmosDbEndpoint");
            KeyVaultEndpoint = nameResolver.Resolve("KeyVaultEndpoint");
        }
    }
}