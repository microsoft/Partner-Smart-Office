// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.WebJobs.Extensions.SmartOffice
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Description;

    /// <summary>
    /// Represents the an attribute used to bind a list of Secure Score entries to a parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class SecureScoreAttribute : TokenBaseAttribute
    {
        /// <summary>
        /// Gets or sets the number of days of score results to retrieve starting from current date.
        /// </summary>
        [AutoResolve]
        [RegularExpression("^[0-9]*$")]
        public string Period { get; set; }
    }
}