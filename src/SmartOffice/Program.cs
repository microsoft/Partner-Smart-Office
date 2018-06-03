// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice
{
    using System;
    using AspNetCore;
    using AspNetCore.Hosting;
    using Azure.KeyVault;
    using Azure.Services.AppAuthentication;
    using Extensions.Configuration;
    using Extensions.Configuration.AzureKeyVault;

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    string keyVaultEndpoint = Environment.GetEnvironmentVariable("KeyVaultEndpoint");

                    if (!string.IsNullOrEmpty(keyVaultEndpoint))
                    {
                        KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                            new AzureServiceTokenProvider().KeyVaultTokenCallback));

                        builder.AddAzureKeyVault(keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                    }
                })
                .UseStartup<Startup>();
    }
}