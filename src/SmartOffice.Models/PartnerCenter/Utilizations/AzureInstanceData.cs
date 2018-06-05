// -----------------------------------------------------------------------
// <copyright file="AzureInstanceData.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Utilizations
{
    using System;
    using System.Collections.Generic; 

    public class AzureInstanceData
    {
        public IDictionary<string, string> AdditionalInfo { get; set; }

        public string Location { get; set; }

        public string OrderNumber { get; set; }

        public string PartNumber { get; set; }


        public Uri ResourceUri { get; set; }

        public IDictionary<string, string> Tags { get; set; }
    }
}