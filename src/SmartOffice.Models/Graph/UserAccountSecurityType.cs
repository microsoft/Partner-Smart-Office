// -----------------------------------------------------------------------
// <copyright file="UserAccountSecurityType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Converters;
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumJsonConverter))]
    public enum UserAccountSecurityType
    {
        Unknown = -1,
        Standard = 0,
        Power = 1,
        Administrator = 2,
        UnknownFutureValue = 127, // 0x0000007F
    }
}