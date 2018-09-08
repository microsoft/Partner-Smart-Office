// -----------------------------------------------------------------------
// <copyright file="LogonType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumConverter))]
    public enum LogonType
    {
        Unknown = 0,
        Interactive = 1,
        RemoteInteractive = 2,
        Network = 3,
        Batch = 4,
        Service = 5,
        UnknownFutureValue = 127
    }
}