// -----------------------------------------------------------------------
// <copyright file="ResourceType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.AuditRecords
{
    using Converters;
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumJsonConverter))]
    public enum ResourceType
    {
        Customer,
        CustomerUser,
        Order,
        Subscription,
        License,
        ThirdPartyAddOn,
        MpnAssociation,
        Transfer,
    }
}