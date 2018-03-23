// -----------------------------------------------------------------------
// <copyright file="IGraphService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Models;

    public interface IGraphService
    {
        Task<List<SecureScore>> GetSecureScoreAsync(int period, CancellationToken cancellationToken = default(CancellationToken));
    }
}