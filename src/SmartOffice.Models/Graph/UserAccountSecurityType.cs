// -----------------------------------------------------------------------
// <copyright file="UserAccountSecurityType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumConverter))]
    public enum UserAccountSecurityType
    {
        Unknown = 0,
        Standard = 1,
        Power = 2,
        Administrator = 3,
        UnknownFutureValue = 127
    }
}