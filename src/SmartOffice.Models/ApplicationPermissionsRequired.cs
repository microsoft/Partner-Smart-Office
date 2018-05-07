// -----------------------------------------------------------------------
// <copyright file="ApplicationPermissionsRequired.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    public enum ApplicationPermissionsRequired
    {
        Unknown = 0,
        Anonymous = 10, // 0x0000000A
        Guest = 20, // 0x00000014
        User = 30, // 0x0000001E
        Administrator = 40, // 0x00000028
        System = 50, // 0x00000032
        UnknownFutureValue = 60, // 0x0000003C
    }
}