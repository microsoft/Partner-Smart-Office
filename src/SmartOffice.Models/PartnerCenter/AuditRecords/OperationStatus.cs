// -----------------------------------------------------------------------
// <copyright file="OperationStatus.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.AuditRecords
{
    using JsonConverters;
    using Newtonsoft.Json; 

    [JsonConverter(typeof(EnumJsonConverter))]
    public enum OperationStatus
    {
        Succeeded,
        Failed,
        Progress,
        Decline,
    }
}