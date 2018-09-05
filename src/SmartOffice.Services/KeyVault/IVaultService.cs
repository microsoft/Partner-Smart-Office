// -----------------------------------------------------------------------
// <copyright file="IVaultSerivce.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.KeyVault
{
    using System.Threading.Tasks;

    public interface IVaultService
    {
        /// <summary>
        /// Deletes the specified secret from the configured key vault.
        /// </summary>
        /// <param name="endpoint">Address for the Azure Key Vault endpoint.</param>
        /// <param name="secretName">The name of the secret.</param>
        /// <returns>
        /// An instance of the <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task DeleteSecretAsync(string endpoint, string secretName);

        /// <summary>
        /// Gets the secret value from the configured instance of Azure Key Vault.
        /// </summary>
        /// <param name="endpoint">Address for the Azure Key Vault endpoint.</param>
        /// <param name="secretName">The name of the secret.</param>
        /// <returns>The value for the speicifed secret.</returns>
        Task<string> GetSecretAsync(string endpoint, string secretName);

        /// <summary>
        /// Sets a secret in the configured key vault. 
        /// </summary>
        /// <param name="endpoint">Address for the Azure Key Vault endpoint.</param>
        /// <param name="secretName">The name of the secret.</param>
        /// <param name="value">The value of the secret.</param>
        /// <param name="contentType">Type of the secret value such as a password.</param>
        /// <returns>
        /// An instance of the <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        Task SetSecretAsync(string endpoint, string secretName, string value, string contentType);
    }
}