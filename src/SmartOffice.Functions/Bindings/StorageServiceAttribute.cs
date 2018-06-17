// -----------------------------------------------------------------------
// <copyright file="StorageServiceAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Bindings
{
    using System;
    using Azure.WebJobs.Description;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class StorageServiceAttribute : Attribute
    {
    }
}