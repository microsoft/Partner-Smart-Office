// -----------------------------------------------------------------------
// <copyright file="ResourceCollection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using PartnerCenter;

    [JsonObject]
    public class ResourceCollection<TResource, TLinks> : ResourceBaseWithLinks<TLinks> where TLinks : new()
    {
        private readonly ICollection<TResource> internalItems;

        public ResourceCollection(ICollection<TResource> items)
          : base("Collection")
        {
            internalItems = items ?? new List<TResource>();
        }

        protected ResourceCollection(ResourceCollection<TResource, TLinks> resourceCollection)
          : base("Collection")
        {
            if (resourceCollection == null)
            {
                throw new ArgumentNullException(nameof(resourceCollection));
            }

            internalItems = resourceCollection.internalItems;
        }

        public IEnumerable<TResource> Items => internalItems;

        [JsonProperty]
        public int TotalCount => internalItems.Count;
    }
}
