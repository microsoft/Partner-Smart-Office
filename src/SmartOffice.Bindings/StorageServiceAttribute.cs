// -----------------------------------------------------------------------
// <copyright file="StorageServiceAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings
{
    using System;
    using Azure.WebJobs;
    using Azure.WebJobs.Description;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class StorageServiceAttribute : Attribute
    {
        public string ConnectionStringName { get; set; }

#pragma warning disable CS0618 // Type or member is obsolete
        [AppSetting(Default = "KeyVaultEndpoint")]
#pragma warning restore CS0618 // Type or member is obsolete
        public string KeyVaultEndpoint { get; set; }
    }
}