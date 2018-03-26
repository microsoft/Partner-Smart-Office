// -----------------------------------------------------------------------
// <copyright file="IPartnerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;
    using Rest;

    public interface IPartnerService
    {
        /// <summary>
        /// Gets or sets the credentials used when accessing resources.
        /// </summary>
        ServiceClientCredentials Credentials { get; }

        /// <summary>
        /// Gets or sets the address of the resource being accessed.
        /// </summary>
        Uri Endpoint { get; }

        /// <summary>
        /// Gets a list of available customers.
        /// </summary>
        /// <param name="cancellationToken">
        /// The token to monitor for cancellation requests
        /// </param>
        /// <returns>A list of available customers.</returns>
        Task<List<Customer>> GetCustomersAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}