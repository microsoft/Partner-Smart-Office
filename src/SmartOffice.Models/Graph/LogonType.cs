// -----------------------------------------------------------------------
// <copyright file="LogonType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    public enum LogonType
    {
        Unknown = -1,
        Interactive = 0,
        RemoteInteractive = 1,
        Network = 2,
        Batch = 3,
        Service = 4,
        UnknownFutureValue = 127, // 0x0000007F
    }
}