// -----------------------------------------------------------------------
// <copyright file="AlertFeedback.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using Newtonsoft.Json; 

    [JsonConverter(typeof(EnumConverter))]
    public enum AlertFeedback
    {
        Unknown = 0,
        TruePositive = 1,
        FalsePositive = 2,
        BenignPositive = 3,
        UnknownFutureValue = 127
    }
}