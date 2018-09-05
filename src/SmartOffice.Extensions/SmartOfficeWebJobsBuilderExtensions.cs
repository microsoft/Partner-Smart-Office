// -----------------------------------------------------------------------
// <copyright file="SmartOfficeWebJobsBuilderExtensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions
{
    using System;
    using Azure.WebJobs;
    using Microsoft.Extensions.DependencyInjection;
    using Services.KeyVault;

    internal static class SmartOfficeWebJobsBuilderExtensions
    {
        public static IWebJobsBuilder AddSmartOffice(this IWebJobsBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddExtension<SmartOfficeExtensionConfig>().BindOptions<SmartOfficeOptions>();

            builder.Services.AddSingleton<IVaultService, KeyVaultService>();

            return builder;
        }
    }
}