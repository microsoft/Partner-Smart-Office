// -----------------------------------------------------------------------
// <copyright file="ProcessIntegrityLevel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    public enum ProcessIntegrityLevel
    {
        Unknown = 0,
        Untrusted = 10, // 0x0000000A
        Low = 20, // 0x00000014
        Medium = 30, // 0x0000001E
        High = 40, // 0x00000028
        System = 50, // 0x00000032
        UnknownFutureValue = 60, // 0x0000003C
    }
}
