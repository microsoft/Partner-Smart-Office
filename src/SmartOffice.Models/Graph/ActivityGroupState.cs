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
        public IEnumerable<string> Aliases { get; set; }

        public string Name { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }
    }
}