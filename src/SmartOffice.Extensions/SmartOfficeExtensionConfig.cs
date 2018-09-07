// -----------------------------------------------------------------------
// <copyright file="SmartOfficeExtensionConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Extensions
{
    using Azure.WebJobs;
    using Azure.WebJobs.Host.Config;
    using Bindings;
    using Converters;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Services.KeyVault;

    public class SmartOfficeExtensionConfig : IExtensionConfigProvider
    {
        /// <summary>
        /// The options available for Partner Smart Office.
        /// </summary>
        private readonly SmartOfficeOptions options;

        private readonly DataRepositoryConverter dataRepoConverter;

        private readonly ILoggerFactory loggerFactory;

        private readonly PartnerServiceConverter partnerServiceConverter;

        private readonly SecureScoreConverter secureScoreConverter;

        private readonly SecurityAlertConverter securityAlertConverter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmartOfficeExtensionConfig" /> class.
        /// </summary>
        /// <param name="nameResolver">Provides the ability to resolve settings.</param>
        /// <param name="options">The options available for Partner Smart Office.</param>
        public SmartOfficeExtensionConfig(ILoggerFactory loggerFactory, INameResolver nameResolver, IOptions<SmartOfficeOptions> options, IVaultService vault)
        {
            this.loggerFactory = loggerFactory;
            this.options = options.Value;
            this.options.SetAppSettings(nameResolver);

            dataRepoConverter = new DataRepositoryConverter(loggerFactory, options, vault);
            partnerServiceConverter = new PartnerServiceConverter(options, vault);
            secureScoreConverter = new SecureScoreConverter(loggerFactory, options, vault);
            securityAlertConverter = new SecurityAlertConverter(loggerFactory, options, vault);
        }

        /// <summary>
        /// Initialize the binding extension
        /// </summary>
        /// <param name="context">Context for the extension</param>
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<DataRepositoryAttribute>().BindToInput(dataRepoConverter);
            context.AddBindingRule<PartnerServiceAttribute>().BindToInput(partnerServiceConverter);
            context.AddBindingRule<SecureScoreAttribute>().BindToInput(secureScoreConverter);
            context.AddBindingRule<SecurityAlertsAttribute>().BindToInput(securityAlertConverter);
        }
    }
}