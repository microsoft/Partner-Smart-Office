// -----------------------------------------------------------------------
// <copyright file="PartnerCenterExtensionConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Bindings
{
    using Azure.WebJobs;
    using Azure.WebJobs.Host;
    using Azure.WebJobs.Host.Config;

    public class PartnerCenterExtensionConfig : IExtensionConfigProvider
    {
        /// <summary>
        /// Used to access application settings for the function application.
        /// </summary>
        private INameResolver appSettings;

        /// <summary>
        /// Used to write to the function application log.
        /// </summary>
        internal TraceWriter log;

        /// <summary>
        /// Initialize the binding extension
        /// </summary>
        /// <param name="context">Context for the extension</param>
        public void Initialize(ExtensionConfigContext context)
        {
            appSettings = context.Config.NameResolver;
            log = context.Trace;
        }
    }
}