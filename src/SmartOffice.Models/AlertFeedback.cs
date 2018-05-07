// -----------------------------------------------------------------------
// <copyright file="AlertFeedback.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    public enum AlertFeedback
    {
        Unknown = 0,
        TruePositive = 1,
        FalsePositive = 2,
        TrueNegative = 3,
        FalseNegative = 4,
        UnknownFutureValue = 127, // 0x0000007F
    }
}