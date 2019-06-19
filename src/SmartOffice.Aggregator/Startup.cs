// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup(typeof(SmartOffice.Aggregator.Startup))]

namespace SmartOffice.Aggregator
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using Data;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Models.Converters;
    using Models.Resolvers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Telemetry;

    /// <summary>
    /// Provides a way to perform dependency injection.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// Configures this instance of Azure Functions.
        /// </summary>
        /// <param name="builder">The instance of <see cref="IFunctionsHostBuilder" /> to configure.</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ITelemetryProvider>(new AppInsightsTelemetryProvider());

            builder.Services.AddSingleton<IDocumentRepository>(s =>
            {
                DbConnectionStringBuilder cnx = new DbConnectionStringBuilder
                {
                    ConnectionString = Environment.GetEnvironmentVariable(OperationConstants.CosmosDbConnectionString)
                };

                cnx.TryGetValue(OperationConstants.AccountEndpoint, out object accountEndpoint);
                cnx.TryGetValue(OperationConstants.AccountKey, out object accountKey);

                return new DocumentRepository(new Uri(accountEndpoint.ToString()), accountKey.ToString());
            });

            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings
                {
                    ContractResolver = new CompositeContractResolver
                    {
                        new CamelCasePropertyNamesContractResolver(),
                        new PrivateContractResolver()
                    },
                    Converters = new List<JsonConverter>
                    {
                        new EnumJsonConverter()
                    },
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                };
            };
        }
    }
}