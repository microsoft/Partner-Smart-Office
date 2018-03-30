// -----------------------------------------------------------------------
// <copyright file="StorageServiceAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings
{
    using System;
    using Azure.WebJobs.Description;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class StorageServiceAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the storage connection string.
        /// </summary>
        public string ConnectionStringName { get; set; }

        /// <summary>
        /// Gets or sets the Azure Key Vault endpoint address.
        /// </summary>
        [AppSetting(Default = "KeyVaultEndpoint")]
        public string KeyVaultEndpoint { get; set; }
    }
}