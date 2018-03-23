// -----------------------------------------------------------------------
// <copyright file="IPartnerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;

    public interface IPartnerService
    {
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