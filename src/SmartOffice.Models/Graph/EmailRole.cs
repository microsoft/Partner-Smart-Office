// -----------------------------------------------------------------------
// <copyright file="EmailRole.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Converters;
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumJsonConverter))]
    public enum EmailRole
    {
        Unknown = 0,
        Sender = 1,
        Recipient = 2,
        UnknownFutureValue = 127
    }
}
