// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using AspNetCore.Authentication.Cookies;
    using AspNetCore.Authentication.OpenIdConnect;
    using AspNetCore.Authorization;
    using AspNetCore.Builder;
    using AspNetCore.Hosting;
    using AspNetCore.Http;
    using AspNetCore.Mvc;
    using AspNetCore.Mvc.Authorization;
    using Authorization;
    using Data;
    using Extensions.Configuration;
    using Extensions.DependencyInjection;
    using IdentityModel.Tokens;
    using Models;
    using Providers;
    using Services;

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

            services.AddApplicationInsightsTelemetry();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                Configuration.Bind("Authentication", options);

                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = async context =>
                    {
                        string signedInUserObjectId = context.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                        string userTenantId = context.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

                        IGraphProvider graph = new GraphProvider(
                            $"{Configuration["ActiveDirectoryEndpoint"]}",
                            Configuration["ApplicationId"],
                            Configuration["ApplicationSecret"],
                            userTenantId);

                        IList<Role> roles = await graph.GetRolesAsync(signedInUserObjectId).ConfigureAwait(false);
                        IList<Claim> claims = roles?.Select(r => new Claim(ClaimTypes.Role, r.DisplayName)).ToList();

                        if (userTenantId.Equals(Configuration["TenantId"], StringComparison.InvariantCultureIgnoreCase))
                        {
                            claims.Add(new Claim("IsPartner", "true"));
                        }

                        context.Principal.AddIdentity(new ClaimsIdentity(claims));
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    SaveSigninToken = true,
                    ValidateIssuer = false
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("PartnerAdmin", policy =>
                    policy.AddRequirements(new PartnerAdminRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, PartnerAdminHandler>();

            services.AddSingleton<IDocumentRepository<EnvironmentDetail>>(
                new DocumentRepository<EnvironmentDetail>(
                    Configuration["CosmosDbEndpoint"],
                    Configuration["CosmosDbAccessKey"],
                    "SmartOffice",
                    "Environments"));

            services.AddSingleton<IVaultService>(
                new KeyVaultService(
                    Configuration["KeyVaultEndpoint"]));

            services.AddMvc(options =>
            {
                AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
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