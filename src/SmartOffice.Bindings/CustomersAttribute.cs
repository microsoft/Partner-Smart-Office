// -----------------------------------------------------------------------
// <copyright file="CustomersAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings
{
    using System;
    using Azure.WebJobs.Description;

    [AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false)]
    [Binding]
    public sealed class CustomersAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the Azure Cosmos DB customers collection identifier.
        /// </summary>
        public string CollectionId { get; set; }

        /// Gets or sets the Azure Cosmos DB database identifier.
        public string DatabaseId { get; set; }
    }
}