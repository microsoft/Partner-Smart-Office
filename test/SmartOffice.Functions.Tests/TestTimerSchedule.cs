// -----------------------------------------------------------------------
// <copyright file="TestTimerSchedule.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Tests
{
    using System;
    using Azure.WebJobs.Extensions.Timers;

    public class TestTimerSchedule : TimerSchedule
    {
        public override DateTime GetNextOccurrence(DateTime now)
        {
            return now.AddDays(1);
        }
    }
}