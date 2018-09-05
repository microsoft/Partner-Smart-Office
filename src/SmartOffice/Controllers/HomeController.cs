// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using AspNetCore.Authorization;
    using AspNetCore.Mvc;
    using Data;
    using Extensions.Configuration;
    using Models;
    using Services.KeyVault;

    [Authorize(Policy = "RequireAdmin")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Provides access to the application configurations.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Data repository used to manage envrionment details.
        /// </summary>
        private readonly IDocumentRepository<EnvironmentDetail> repository;

        /// <summary>
        /// Provides access to an instance of Azure Key Vault.
        /// </summary>
        private readonly IVaultService vault;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="configuration">Provides access to the application configurations.</param>
        /// <param name="repository">Data repository used to manage envrionment details.</param>
        /// <param name="vault">Provides access to an instance of Azure Key Vault.</param>
        public HomeController(IConfiguration configuration, IDocumentRepository<EnvironmentDetail> repository, IVaultService vault)
        {
            this.configuration = configuration;
            this.repository = repository;
            this.vault = vault;
        }

        public IActionResult AddNew()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNew(
            [Bind("AppEndpoint,EnvironmentType,FriendlyName,Id,PartnerCenterEndpoint,ProcessAzureUsage")] EnvironmentDetail environment)
        {
            if (ModelState.IsValid)
            {
                environment.Modified = DateTimeOffset.Now;

                if (!string.IsNullOrEmpty(environment.AppEndpoint?.ApplicationSecret))
                {
                    await SetSecretAsync(Guid.NewGuid().ToString(), environment.AppEndpoint).ConfigureAwait(false);
                }

                if (!string.IsNullOrEmpty(environment.PartnerCenterEndpoint?.ApplicationSecret))
                {
                    await SetSecretAsync(Guid.NewGuid().ToString(), environment.PartnerCenterEndpoint).ConfigureAwait(false);
                }

                await repository.AddOrUpdateAsync(environment).ConfigureAwait(false);

                return RedirectToAction("Index");
            }

            return View(environment);
        }

        public async Task<IActionResult> Delete(string id)
        {
            EnvironmentDetail environment;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                environment = await repository.GetAsync(id).ConfigureAwait(false);

                if (environment == null)
                {
                    return NotFound();
                }

                return View(environment);
            }
            finally
            {
                environment = null;
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            EnvironmentDetail environment;

            try
            {
                environment = await repository.GetAsync(id).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(environment.AppEndpoint?.ApplicationSecretId))
                {
                    // Remove the secret associated with the Azure AD application from key vault.
                    await vault.DeleteSecretAsync(
                        configuration["KeyVaultEndpoint"], 
                        environment.AppEndpoint.ApplicationSecretId).ConfigureAwait(false);
                }

                if (!string.IsNullOrEmpty(environment.PartnerCenterEndpoint?.ApplicationSecretId))
                {
                    // Remove the secret associated with the Partner Center application from key vault.
                    await vault.DeleteSecretAsync(
                        configuration["KeyVaultEndpoint"],
                        environment.PartnerCenterEndpoint.ApplicationSecretId).ConfigureAwait(false);
                }

                await repository.DeleteAsync(id).ConfigureAwait(false);

                return RedirectToAction("Index");
            }
            finally
            {
                environment = null;
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            EnvironmentDetail environment;

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                environment = await repository.GetAsync(id).ConfigureAwait(false);

                if (environment == null)
                {
                    return NotFound();
                }

                return View(environment);
            }
            finally
            {
                environment = null;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            string id,
            [Bind("AppEndpoint,EnvironmentType,FriendlyName,Id,PartnerCenterEndpoint,ProcessAzureUsage")] EnvironmentDetail environment)
        {
            EnvironmentDetail current;

            try
            {
                if (id != environment.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    current = await repository.GetAsync(id).ConfigureAwait(false);

                    if (current == null)
                    {
                        return NotFound();
                    }

                    current.AppEndpoint = environment.AppEndpoint;
                    current.EnvironmentType = environment.EnvironmentType;
                    current.FriendlyName = environment.FriendlyName;
                    current.Modified = DateTimeOffset.UtcNow;
                    current.PartnerCenterEndpoint = environment.PartnerCenterEndpoint;

                    if (!string.IsNullOrEmpty(current.AppEndpoint?.ApplicationSecret))
                    {
                        await SetSecretAsync(
                            current.AppEndpoint.ApplicationSecretId,
                            current.AppEndpoint).ConfigureAwait(false);
                    }

                    if (!string.IsNullOrEmpty(current.PartnerCenterEndpoint?.ApplicationSecret))
                    {
                        await SetSecretAsync(
                            current.PartnerCenterEndpoint.ApplicationSecretId,
                            current.PartnerCenterEndpoint).ConfigureAwait(false);
                    }

                    await repository.UpdateAsync(current).ConfigureAwait(false);

                    return RedirectToAction("Index");
                }

                return View(environment);
            }
            finally
            {
                current = null;
            }
        }

        public async Task<IActionResult> Index()
        {
            List<EnvironmentDetail> environments;

            try
            {
                await repository.InitializeAsync().ConfigureAwait(false);

                environments = await repository.GetAsync().ConfigureAwait(false);

                return View(environments);
            }
            finally
            {
                environments = null;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task SetSecretAsync(string secretName, EndpointDetail endpoint)
        {
            if (string.IsNullOrEmpty(secretName))
            {
                secretName = Guid.NewGuid().ToString();
            }

            endpoint.ApplicationSecretId = secretName;

            await vault.SetSecretAsync(
                configuration["KeyVaultEndpoint"],
                endpoint.ApplicationSecretId,
                endpoint.ApplicationSecret,
                "text/plain").ConfigureAwait(false);

            endpoint.ApplicationSecret = null;
        }
    }
}