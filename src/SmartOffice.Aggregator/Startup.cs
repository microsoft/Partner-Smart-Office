// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup(typeof(SmartOffice.Aggregator.Startup))]

namespace SmartOffice.Aggregator
{
    using System;
    using System.Data.Common;
    using Data;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IDocumentRepository>(s =>
            {
                DbConnectionStringBuilder cnx = new DbConnectionStringBuilder
                {
                    ConnectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString")
                };

                cnx.TryGetValue("AccountEndpoint", out object accountEndpoint);
                cnx.TryGetValue("AccountKey", out object accountKey);

                return new DocumentRepository(new Uri(accountEndpoint.ToString()), accountKey.ToString());
            });


            JsonConvert.DefaultSettings = () =>
            {
                return new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                };
            };
        }
    }
}