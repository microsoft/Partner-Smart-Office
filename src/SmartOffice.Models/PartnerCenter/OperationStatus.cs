// -----------------------------------------------------------------------
// <copyright file="OperationStatus.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter
{
    using Converters;
    using Newtonsoft.Json;

    public enum OperationStatus
    {
        Succeeded,
        Failed,
        Progress,
        Decline,
    }
}