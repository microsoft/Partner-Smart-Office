// -----------------------------------------------------------------------
// <copyright file="IGraphService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Models.Graph;
    using Rest;

    public interface IGraphService
    {
        /// <summary>
        /// Gets the credentials used when accessing resources.
        /// </summary>
        ServiceClientCredentials Credentials { get; }

        /// <summary>
        /// Gets or sets the address of the resource being accessed.
        /// </summary>
        Uri Endpoint { get; }

        Task<List<Alert>> GetAlertsAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the secure score for the defined period.
        /// </summary>
        /// <param name="period">Number of days of score results to retrieve starting from current date.</param>
        /// <param name="cancellationToken">The cancellation token to monitor.</param>
        /// <returns>A list of secure scores for the defined period.</returns>
        Task<List<SecureScore>> GetSecureScoreAsync(int period, CancellationToken cancellationToken = default(CancellationToken));
    }
}