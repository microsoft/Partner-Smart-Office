// -----------------------------------------------------------------------
// <copyright file="IHttpService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;
    using System.Threading.Tasks; 

    public interface IHttpService
    {
        Task<TResponse> GetAsync<TResponse>(Uri requestUri, IRequestContext requestContext);
    }
}