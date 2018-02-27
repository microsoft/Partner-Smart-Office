// -----------------------------------------------------------------------
// <copyright file="TokenAttribute.cs" company="Microsoft">
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
    public sealed class TokenAttribute : Attribute
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [AppSetting(Default = "ApplicationId")]
#pragma warning restore CS0618 // Type or member is obsolete

        public string ApplicationId { get; set; }

        public string AppSecretId { get; set; }

#pragma warning disable CS0618 // Type or member is obsolete
        [AutoResolve]
        public string ApplicationTenantId { get; set; }
#pragma warning restore CS0618 // Type or member is obsolete

        public string Resource { get; set; }
    }
}