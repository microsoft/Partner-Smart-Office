// -----------------------------------------------------------------------
// <copyright file="ErrorViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EnvironmentViewModel
    {
        private readonly IList<SelectListItem> environmentTypes;

        public EnvironmentViewModel()
        {
            environmentTypes = new List<SelectListItem>
            {
                new SelectListItem { Value = "CSP", Text = "CSP" },
                new SelectListItem { Value = "EA", Text = "EA" }
            };
        }
    }
}