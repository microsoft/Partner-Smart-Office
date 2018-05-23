// -----------------------------------------------------------------------
// <copyright file="AlertType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    public enum AlertType
    {
        Unknown = 0,
        SingleSensorAtomic = 10, // 0x0000000A
        SingleSensorCompound = 20, // 0x00000014
        MultiSensorCompound = 30, // 0x0000001E
        UnknownFutureValue = 127, // 0x0000007F
    }
}