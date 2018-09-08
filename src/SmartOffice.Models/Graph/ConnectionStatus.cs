// -----------------------------------------------------------------------
// <copyright file="ConnectionStatus.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumConverter))]
    public enum ConnectionStatus
    {
        Unknown = 0,
        Attempted = 1,
        Succeeded = 2,
        Blocked = 3,
        Failed = 4,
        UnknownFutureValue = 127
    }
}