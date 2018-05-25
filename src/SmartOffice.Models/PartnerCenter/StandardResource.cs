// -----------------------------------------------------------------------
// <copyright file="StandardResource.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter
{
    using Newtonsoft.Json;

    public class StandardResource : ResourceBaseWithLinks<StandardResourceLinks>
    {
        /// <summary>
        /// Gets or sets the identifier for the resource.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
