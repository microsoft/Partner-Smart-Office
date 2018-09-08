// -----------------------------------------------------------------------
// <copyright file="FileHashType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumConverter))]
    public enum FileHashType
    {
        Unknown = 0,
        Sha1 = 1,
        Sha256 = 2,
        Md5 = 3,
        AuthenticodeHash256 = 4,
        LsHash = 5,
        Ctph = 6,
        PeSha1 = 7,
        PeSha256 = 8,
        UnknownFutureValue = 127
    }
}