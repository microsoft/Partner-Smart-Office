// -----------------------------------------------------------------------
// <copyright file="Resources.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Resources<TEntity>
    {
        [JsonProperty(PropertyName = "items")]
        public List<TEntity> Items { get; set; }


        [JsonProperty(PropertyName = "totalCount")]
        public int TotalCount { get; set; }
    }
}