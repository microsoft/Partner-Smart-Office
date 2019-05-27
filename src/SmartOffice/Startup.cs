// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.AzureAD.UI;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;
    using Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Instead of using the default validation (validating against a single issuer value, as we do in
                    // line of business apps), we inject our own multitenant validation logic
                    ValidateIssuer = false,

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                        // If your authentication logic is based on users then add your logic here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    },
                    // If your application needs to do authenticate single users, add your user validation below.
                    //OnTokenValidated = context =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                };
            });

            // The following line enables Application Insights telemetry collection.
            services.AddApplicationInsightsTelemetry();

            services.AddMvc(options =>
            {
                AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IDataRepository<EnvironmentEntry>>(repo => new DataRepository<EnvironmentEntry>(
                new Uri(Configuration["CosmosDbEndpoint"]),
                Configuration["CosmosDbAccessKey"],
                "smartoffice",
                "environments"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}