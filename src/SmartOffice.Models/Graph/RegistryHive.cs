// -----------------------------------------------------------------------
// <copyright file="RegistryHive.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json;

    [JsonConverter(typeof(EnumConverter))]
    public enum RegistryHive
    {
        Unknown = 0,
        CurrentConfig = 1,
        CurrentUser = 2,
        LocalMachineSam = 3,
        LocalMachineSamSoftware = 4,
        LocalMachineSystem = 5,
        UsersDefault = 6,
        UnknownFutureValue = 127
    }
}