// -----------------------------------------------------------------------
// <copyright file="ResourceType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter
{
    using Converters;
    using Newtonsoft.Json; 

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