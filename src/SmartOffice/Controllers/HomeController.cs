// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;

    [Authorize]
    public class HomeController : Controller
    {
        private readonly IDataRepository<EnvironmentEntry> repository;

        public HomeController(IDataRepository<EnvironmentEntry> repository)
        {
            this.repository = repository;
        }

        public async Task<IActionResult> Delete(string id)
        {
            EnvironmentEntry environment;

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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            EnvironmentEntry environment = await repository.GetAsync(id).ConfigureAwait(false);

            //if (!string.IsNullOrEmpty(environment.AppEndpoint?.ApplicationSecretId))
            //{
            //    // Remove the secret associated with the Azure AD application from key vault.
            //    await vault.DeleteSecretAsync(
            //        configuration["KeyVaultEndpoint"],
            //        environment.AppEndpoint.ApplicationSecretId).ConfigureAwait(false);
            //}

            //if (!string.IsNullOrEmpty(environment.PartnerCenterEndpoint?.ApplicationSecretId))
            //{
            //    // Remove the secret associated with the Partner Center application from key vault.
            //    await vault.DeleteSecretAsync(
            //        configuration["KeyVaultEndpoint"],
            //        environment.PartnerCenterEndpoint.ApplicationSecretId).ConfigureAwait(false);
            //}

            await repository.DeleteAsync(id).ConfigureAwait(false);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            EnvironmentEntry environment;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AppEndpoint,EnvironmentType,FriendlyName,Id,PartnerCenterEndpoint")] EnvironmentEntry environment)
        {
            EnvironmentEntry current;

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
                current.FriendlyName = environment.FriendlyName;
                current.PartnerCenterEndpoint = environment.PartnerCenterEndpoint;

                //if (!string.IsNullOrEmpty(current.AppEndpoint?.ApplicationSecret))
                //{
                //    await SetSecretAsync(
                //        current.AppEndpoint.ApplicationSecretId,
                //        current.AppEndpoint).ConfigureAwait(false);
                //}

                //if (!string.IsNullOrEmpty(current.PartnerCenterEndpoint?.ApplicationSecret))
                //{
                //    await SetSecretAsync(
                //        current.PartnerCenterEndpoint.ApplicationSecretId,
                //        current.PartnerCenterEndpoint).ConfigureAwait(false);
                //}

                await repository.UpdateAsync(current).ConfigureAwait(false);

                return RedirectToAction("Index");
            }

            return View(environment);
        }

        public async Task<IActionResult> Index()
        {
            List<EnvironmentEntry> environments = await repository.GetAsync().ConfigureAwait(false);

            return View(environments);
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
    }
}