// -----------------------------------------------------------------------
// <copyright file="DataRepositoryAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions.Bindings
{
    using System;
    using Azure.WebJobs.Description;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class DataRepositoryAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the data type of the repository. 
        /// </summary>
        public Type DataType { get; set; }
    }
}