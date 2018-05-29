// -----------------------------------------------------------------------
// <copyright file="DataRepositoryAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Bindings
{
    using System;
    using Azure.WebJobs.Description;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class DataRepositoryAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the Azure Cosmos DB endpoint address.
        /// </summary>
        [AppSetting(Default = "CosmosDbEndpoint")]
        public string CosmosDbEndpoint { get; set; }

        /// <summary>
        /// Gets or sets the data type of the repository. 
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets the Azure Key Vault endpoint address.
        /// </summary>
        [AppSetting(Default = "KeyVaultEndpoint")]
        public string KeyVaultEndpoint { get; set; }
    }
}