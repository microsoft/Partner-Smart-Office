// -----------------------------------------------------------------------
// <copyright file="AlertTrigger.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.Collections.Generic; 

    public class AlertTrigger
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }
    }
}