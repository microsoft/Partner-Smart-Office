// -----------------------------------------------------------------------
// <copyright file="AlertStatus.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    public enum AlertStatus
    {
        Unknown = 0,
        NewAlert = 10, // 0x0000000A
        InProgress = 20, // 0x00000014
        Resolved = 30, // 0x0000001E
        UnknownFutureValue = 127, // 0x0000007F
    }
}