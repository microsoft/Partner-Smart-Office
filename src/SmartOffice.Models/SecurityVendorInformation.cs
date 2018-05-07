// -----------------------------------------------------------------------
// <copyright file="SecurityVendorInformation.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.Collections.Generic;

    public class SecurityVendorInformation
    {
        public string Provider { get; set; }

        public string ProviderVersion { get; set; }

        public string SubProvider { get; set; }

        public string Vendor { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }
    }
}