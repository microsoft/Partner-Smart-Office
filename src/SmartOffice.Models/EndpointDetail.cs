// -----------------------------------------------------------------------
// <copyright file="EndpointDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.ComponentModel.DataAnnotations;

    public sealed class EndpointDetail
    {
        /// <summary>
        /// Gets or sets identifier of the application.
        /// </summary>
        [Display(Name = "Application Identifier")]
        [RegularExpression(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$")]
        [Required]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the application secret.
        /// </summary>
        [Display(Name = "Secret Identifier")]
        [Required]
        public string ApplicationSecretId { get; set; }

        /// <summary>
        /// Gets or sets the service address.
        /// </summary>
        [Display(Name = "Service Address")]
        [Required]
        public string ServiceAddress { get; set; }

        /// <summary>
        /// Gets or set the identifier of the tenant.
        /// </summary>
        [Display(Name = "Tenant Identifier")]
        [RegularExpression(@"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$")]
        [Required]
        public string TenantId { get; set; }
    }
}