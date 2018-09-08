// -----------------------------------------------------------------------
// <copyright file="BillingType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Subscriptions
{
    using JsonConverters;
    using Newtonsoft.Json; 

    [JsonConverter(typeof(EnumJsonConverter))]
    public enum BillingType
    {
        None,
        Usage,
        License,
    }
}