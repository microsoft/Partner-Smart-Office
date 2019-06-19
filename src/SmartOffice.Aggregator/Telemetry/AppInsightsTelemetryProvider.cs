// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Telemetry
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;

    /// <summary>
    /// Provides the ability to capture telemetry using Application Insights.
    /// </summary>
    public class AppInsightsTelemetryProvider : ITelemetryProvider
    {
        /// <summary>
        /// Used to send events, metrics, and other telemetry to Application Insights.
        /// </summary>
        private readonly TelemetryClient telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppInsightsTelemetryProvider" /> class.
        /// </summary>
        public AppInsightsTelemetryProvider()
        {
            telemetry = new TelemetryClient();
        }

        /// <summary>
        /// Sends an event for display in the diagnostic search and aggregation in the metrics explorer.
        /// </summary>
        /// <param name="eventName">A name for the event.</param>
        /// <param name="properties">Named string values you can use to search and classify events.</param>
        /// <param name="metrics">Measurements associated with this event.</param>
        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            telemetry.TrackEvent(eventName, properties, metrics);
        }

        /// <summary>
        /// Sends an exception for display the in diagnostic search.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        /// <param name="properties">Named string values you can use to classify and search for this exception.</param>
        /// <param name="metrics">Additional values associated with this exception.</param>
        public void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            telemetry.TrackException(exception, properties, metrics);
        }

        /// <summary>
        /// Send a trace message for display in the diagnostic search.
        /// </summary>
        /// <param name="message">The message to be tracked.</param>
        public void TrackTrace(string message)
        {
            telemetry.TrackTrace(message);
        }
    }
}