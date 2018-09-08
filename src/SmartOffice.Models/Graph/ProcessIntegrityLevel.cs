// -----------------------------------------------------------------------
// <copyright file="ProcessIntegrityLevel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumConverter))]
    public enum ProcessIntegrityLevel
    {
        Unknown = 0,
        Untrusted = 1,
        Low = 2,
        Medium = 3,
        High = 4,
        System = 5,
        UnknownFutureValue = 127
    }
}
