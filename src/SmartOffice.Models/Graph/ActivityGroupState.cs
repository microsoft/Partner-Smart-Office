// -----------------------------------------------------------------------
// <copyright file="ActivityGroupState.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System.Collections.Generic;

    public class ActivityGroupState
    {
        /// <summary>
        /// Gets or sets a collection aliases.
        /// </summary>
        public IEnumerable<string> Aliases { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a collection of additional data.
        /// </summary>
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}