// -----------------------------------------------------------------------
// <copyright file="AlertStatus.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    public enum AlertStatus
    {
        Unknown = 0,
        NewAlert = 1,
        InProgress = 2,
        Resolved = 3,
        Dismissed = 4,
        UnknownFutureValue = 127
    }
}