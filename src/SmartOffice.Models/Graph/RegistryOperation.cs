// -----------------------------------------------------------------------
// <copyright file="RegistryOperation.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumConverter))]
    public enum RegistryOperation
    {
        Unknown = 0,
        Create = 1,
        Modify = 2,
        Delete = 3,
        UnknownFutureValue = 127
    }
}