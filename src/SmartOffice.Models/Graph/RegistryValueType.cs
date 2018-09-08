// -----------------------------------------------------------------------
// <copyright file="RegistryValueType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumConverter))]
    public enum RegistryValueType
    {
        Unknown = 0,
        Binary = 1,
        Dword = 2,
        DwordLittleEndian = 3,
        DwordBigEndian = 4,
        ExpandSz = 5,
        Link = 6,
        MultiSz = 7,
        None = 8,
        Qword = 9,
        QwordlittleEndian = 10,
        Sz = 11,
        UnknownFutureValue = 127
    }
}