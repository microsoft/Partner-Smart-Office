// -----------------------------------------------------------------------
// <copyright file="TestTraceWriter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Azure.WebJobs.Host;

    internal class TestTraceWriter : TraceWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestTraceWriter" />
        /// </summary>
        public TestTraceWriter() : base(TraceLevel.Verbose)
        {
            TraceEvents = new List<TraceEvent>();
        }

        internal List<TraceEvent> TraceEvents { get; }

        public override void Trace(TraceEvent traceEvent)
        {
            TraceEvents.Add(traceEvent);
        }
    }
}