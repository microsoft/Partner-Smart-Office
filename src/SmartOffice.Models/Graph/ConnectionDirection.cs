// -----------------------------------------------------------------------
// <copyright file="ConnectionDirection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Converters;
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumJsonConverter))]
    public enum ConnectionDirection
    {
        Unknown = 0,
        Inbound = 1,
        Outbound = 2,
        UnknownFutureValue = 127
    }
}
