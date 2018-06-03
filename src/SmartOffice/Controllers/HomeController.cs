// -----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using AspNetCore.Authorization;
    using AspNetCore.Mvc;
    using Data;
    using Models;

    [Authorize]
    public class HomeController : Controller
    {
        /// <summary>
        /// Data repository used to manage envrionment details.
        /// </summary>
        private readonly IDocumentRepository<EnvironmentDetail> repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController" /> class.
        /// </summary>
        /// <param name="repository">Data repository used to manage envrionment details.</param>
        public HomeController(IDocumentRepository<EnvironmentDetail> repository)
        {
            this.repository = repository;
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
            await repository.DeleteAsync(id).ConfigureAwait(false); 
            return RedirectToAction("Index");
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
            [Bind("AppEndpoint,EnvironmentType,FriendlyName,Id,PartnerCenterEndpoint")] EnvironmentDetail environment)
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
                    current.PartnerCenterEndpoint = environment.PartnerCenterEndpoint;

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
    }
}