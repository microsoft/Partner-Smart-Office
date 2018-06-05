// -----------------------------------------------------------------------
// <copyright file="IGraphProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Providers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface IGraphProvider
    {
        Task<IList<Role>> GetRolesAsync(string objectId);
    }
}