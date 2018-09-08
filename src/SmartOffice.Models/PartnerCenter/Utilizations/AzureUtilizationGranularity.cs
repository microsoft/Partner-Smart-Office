// -----------------------------------------------------------------------
// <copyright file="AzureUtilizationGranularity.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Utilizations
{
    using JsonConverters;
    using Newtonsoft.Json; 

    [JsonConverter(typeof(EnumJsonConverter))]
    public enum AzureUtilizationGranularity
    {
        Daily,
        Hourly,
    }
}