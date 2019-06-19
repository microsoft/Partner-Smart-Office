// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.AzureMonitor;
    using Microsoft.Azure.WebJobs.Extensions.AzureSecurity;
    using Microsoft.Azure.WebJobs.Extensions.MicrosoftGraph;
    using Microsoft.Extensions.Logging;
    using Microsoft.Graph;
    using Models;
    using Telemetry;

    /// <summary>
    /// Defines the Azure Functions used to process security events.
    /// </summary>
    public class SecurityEvents
    {
        /// <summary>
        /// Provides the ability to manage a document repository.
        /// </summary>
        private readonly IDocumentRepository repository;

        /// <summary>
        /// Provides the ability to capture telemetry.
        /// </summary>
        private readonly ITelemetryProvider telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityEvents" /> class.
        /// </summary>
        /// <param name="repository">Provides the ability to manage a document repository.</param>
        /// <param name="telemetry">Provides the ability to capture telemetry.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="repository"/> is null
        /// or
        /// <paramref name="telemetry"/> is null
        /// </exception>
        public SecurityEvents(IDocumentRepository repository, ITelemetryProvider telemetry)
        {
            repository.AssertNotNull(nameof(repository));
            telemetry.AssertNotNull(nameof(telemetry));

            this.repository = repository;
            this.telemetry = telemetry;
        }

        /// <summary>
        /// Synchronizes Secure Score control profile information for the specified customer.
        /// </summary>
        /// <param name="input">The instance of <see cref="CustomerRecord" /> that triggered the execution.</param>
        /// <param name="profiles">A collection of Secure Score control profiles associated with the customer.</param>
        /// <param name="log">Provides the ability to log diagnostic information.</param>
        /// <returns>An instance of the <see cref="Task" /> class that represents the asynchronous operation.</returns>
        [FunctionName(OperationConstants.ControlProfileSync)]
        public async Task ControlProfileSyncAsync(
            [QueueTrigger(OperationConstants.ControlProfileSync, Connection = OperationConstants.StorageConnectionString)]CustomerRecord input,
            [SecureScoreControlProfile(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                ApplicationSecretName = "{AppEndpoint.ApplicationSecretName}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                TenantId = "{Id}")]List<SecureScoreControlProfile> profiles,
            ILogger log)
        {
            DateTime executionTime = DateTime.UtcNow;

            log.LogInformation($"Processing {profiles.Count} Secure Score control profiles for {input.Entry.CustomerName}");

            await repository.AddOrUpdateAsync(
                OperationConstants.DatabaseId,
                OperationConstants.SecurityEventsCollectionId,
                input.Entry.CustomerId,
                new CustomerDataEntry<List<SecureScoreControlProfile>>
                {
                    CustomerId = input.Entry.CustomerId,
                    CustomerName = input.Entry.CustomerName,
                    Entry = profiles,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = $"{input.Entry.CustomerId}-controlprofile"
                }).ConfigureAwait(false);

            Dictionary<string, double> eventMetrics = new Dictionary<string, double>
            {
                { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds },
                { "NumberOfSecureScoreControlProfiles", profiles.Count }
            };

            Dictionary<string, string> eventProperties = new Dictionary<string, string>
            {
                { "CustomerId", input.Entry.CustomerId },
                { "EnvironmentId", input.EnvironmentId }
            };

            telemetry.TrackEvent(nameof(ControlProfileSyncAsync), eventProperties, eventMetrics);
        }

        [FunctionName(OperationConstants.SecurityEventSync)]
        public async Task SecurityEventSyncAsync(
            [QueueTrigger(OperationConstants.SecurityEventSync, Connection = OperationConstants.StorageConnectionString)]CustomerRecord input,
            [Assessment(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                ApplicationSecretName = "{AppEndpoint.ApplicationSecretName}",
                Authority = "https://login.microsoftonline.com/{Id}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                RefreshTokenName = "{AppEndpoint.RefreshTokenName}",
                Subscriptions = "{Subscriptions}",
                TenantId = "{Id}")]List<Assessment> assessments,
            [DirectoryAudit(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                ApplicationSecretName = "{AppEndpoint.ApplicationSecretName}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                TenantId = "{Id}")]List<DirectoryAudit> logs,
            [SecureScore(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                ApplicationSecretName = "{AppEndpoint.ApplicationSecretName}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                Period = "{SecureScorePeriod}",
                TenantId = "{Id}")]List<SecureScore> scores,
            [SecurityAlert(
                ApplicationId = "{AppEndpoint.ApplicationId}",
                ApplicationSecretName = "{AppEndpoint.ApplicationSecretName}",
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                TenantId = "{Id}")]List<Alert> alerts,
            [DataCollector(
                KeyVaultEndpoint = "%KeyVaultEndpoint%",
                WorkspaceId = "{WorkspaceId}",
                WorkspaceKeyName = "{WorkspaceKeyName}")]IAsyncCollector<LogData> datacCollector,
            ILogger log)
        {
            DateTime executionTime = DateTime.UtcNow;

            log.LogInformation($"Processing security events for {input.Id}");

            await repository.AddOrUpdateAsync(
                OperationConstants.DatabaseId,
                OperationConstants.SecurityEventsCollectionId,
                input.Entry.CustomerId,
                assessments.Select(entry => new CustomerDataEntry<Assessment>
                {
                    CustomerId = input.Entry.CustomerId,
                    CustomerName = input.Entry.CustomerName,
                    Entry = entry,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = $"{entry.AssessmentKey}_{DateTime.Now.ToString("yyyy-MM-dd")}"
                })).ConfigureAwait(false);

            await datacCollector.AddAsync(new LogData
            {
                Data = logs,
                LogType = nameof(DirectoryAudit),
                TimeStampField = "ActivityDateTime"
            }).ConfigureAwait(false);

            await repository.AddOrUpdateAsync(
                OperationConstants.DatabaseId,
                OperationConstants.SecurityEventsCollectionId,
                input.Entry.CustomerId,
                logs.Select(entry => new CustomerDataEntry<DirectoryAudit>
                {
                    CustomerId = input.Entry.CustomerId,
                    CustomerName = input.Entry.CustomerName,
                    Entry = entry,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = entry.Id
                })).ConfigureAwait(false);

            await datacCollector.AddAsync(new LogData
            {
                Data = alerts,
                LogType = nameof(Alert),
                TimeStampField = "CreatedDateTime"
            }).ConfigureAwait(false);

            await repository.AddOrUpdateAsync(
                OperationConstants.DatabaseId,
                OperationConstants.SecurityEventsCollectionId,
                input.Entry.CustomerId,
                alerts.Select(entry => new CustomerDataEntry<Alert>
                {
                    CustomerId = input.Entry.CustomerId,
                    CustomerName = input.Entry.CustomerName,
                    Entry = entry,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = entry.Id
                })).ConfigureAwait(false);

            await datacCollector.AddAsync(new LogData
            {
                Data = scores,
                LogType = nameof(SecureScore),
                TimeStampField = "CreatedDateTime"
            }).ConfigureAwait(false);

            await repository.AddOrUpdateAsync(
                OperationConstants.DatabaseId,
                OperationConstants.SecurityEventsCollectionId,
                input.Entry.CustomerId,
                scores.Select(entry => new CustomerDataEntry<SecureScore>
                {
                    CustomerId = input.Entry.CustomerId,
                    CustomerName = input.Entry.CustomerName,
                    Entry = entry,
                    EnvironmentId = input.EnvironmentId,
                    EnvironmentName = input.EnvironmentName,
                    Id = entry.Id
                })).ConfigureAwait(false);

            Dictionary<string, double> eventMetrics = new Dictionary<string, double>
            {
                { "ElapsedMilliseconds", DateTime.Now.Subtract(executionTime).TotalMilliseconds },
                { "NumberOfAssessments", assessments.Count },
                { "NumberOfDirectoryAuditLog", logs.Count },
                { "NumberOfSecurityAlerts", alerts.Count },
                { "NumberOfSecureScoreEntries", scores.Count }
            };

            Dictionary<string, string> eventProperties = new Dictionary<string, string>
            {
                { "CustomerId", input.Entry.CustomerId },
                { "EnvironmentId", input.EnvironmentId }
            };

            telemetry.TrackEvent(nameof(SecurityEventSyncAsync), eventProperties, eventMetrics);
        }
    }
}