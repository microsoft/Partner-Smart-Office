// -----------------------------------------------------------------------
// <copyright file="SecureScoreAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Azure.WebJobs.Description;

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class SecureScoreAttribute : TokenBaseAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating number of days of score results to retrieve starting from current date.
        /// </summary>
        [AutoResolve]
        [RegularExpression("^[0-9]*$")]
        public string Period { get; set; }
    }
}