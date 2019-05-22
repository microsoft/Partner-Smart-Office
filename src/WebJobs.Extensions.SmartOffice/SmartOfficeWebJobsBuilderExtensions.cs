// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.WebJobs.Extensions.SmartOffice
{
    /// <summary>
    /// Provides useful extension methods for enable the Smart Office extension.
    /// </summary>
    public static class SmartOfficeWebJobsBuilderExtensions
    {
        /// <summary>
        /// Adds the Partner Center extension to the provided <see cref="IWebJobsBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="IWebJobsBuilder" /> to configure.</param>
        public static IWebJobsBuilder AddPartnerCenter(this IWebJobsBuilder builder)
        {
            builder.AssertNotNull(nameof(builder));

            builder.AddExtension<SmartOfficeExtensionConfigProvider>();

            return builder;
        }
    }
}