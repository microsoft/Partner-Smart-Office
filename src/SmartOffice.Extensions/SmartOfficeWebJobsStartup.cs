// -----------------------------------------------------------------------
// <copyright file="SmartOfficeWebJobsStartup.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Microsoft.Partner.SmartOffice.Extensions.SmartOfficeWebJobsStartup))]

namespace Microsoft.Partner.SmartOffice.Extensions
{
    using Azure.WebJobs;
    using Azure.WebJobs.Hosting;
    using Bindings;

    public class SmartOfficeWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddSmartOffice();
        }
    }
}