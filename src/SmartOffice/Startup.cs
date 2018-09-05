// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice
{
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
    using Services.KeyVault;

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

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    SaveSigninToken = true,
                    ValidateIssuer = false
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy =>
                    policy.AddRequirements(new SmartOfficeAdminRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, SmartOfficeAdminHandler>();

            services.AddSingleton<IDocumentRepository<EnvironmentDetail>>(
                new DocumentRepository<EnvironmentDetail>(
                    Configuration["CosmosDbEndpoint"],
                    Configuration["CosmosDbAccessKey"],
                    "SmartOffice",
                    "Environments"));

            services.AddSingleton<IVaultService, KeyVaultService>();

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