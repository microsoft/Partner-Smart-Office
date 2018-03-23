// -----------------------------------------------------------------------
// <copyright file="IVaultSerivce.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System.Threading.Tasks;

    public interface IVaultService
    {
        /// <summary>
        /// Gets the secret value from the configured instance of Azure Key Vault.
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be retrieved.</param>
        /// <returns>The value for the speicifed secret.</returns>
        Task<string> GetSecretAsync(string identifier);
    }
}