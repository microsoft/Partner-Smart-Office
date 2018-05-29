// -----------------------------------------------------------------------
// <copyright file="StandardResourceLinks.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter
{
    using Newtonsoft.Json;

    public class StandardResourceLinks
    {
        /// <summary>
        /// Gets or sets the self link.
        /// </summary>
        [JsonProperty]
        public Link Self { get; set; }
    }
}
