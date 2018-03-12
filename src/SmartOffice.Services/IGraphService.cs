// -----------------------------------------------------------------------
// <copyright file="IGraphService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IGraphService
    {
        Task<List<SecureScore>> GetSecureScoreAsync(IRequestContext requestContext, int period);
    }
}