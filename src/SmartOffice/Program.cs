// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.AzureKeyVault;

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (context.HostingEnvironment.IsProduction())
                    {
                        IConfigurationRoot builtConfig = config.Build();

                        KeyVaultClient keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                new AzureServiceTokenProvider().KeyVaultTokenCallback));

                        config.AddAzureKeyVault(
                            builtConfig["KeyVaultEndpoint"],
                            keyVaultClient,
                            new DefaultKeyVaultSecretManager());
                    }
                })
                .UseApplicationInsights()
                .UseStartup<Startup>();
    }
}