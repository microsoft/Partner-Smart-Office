// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Models
{
    /// <summary>
    /// Represents metadata for endpoints.
    /// </summary>
    public sealed class EndpointEntry
    {
        /// <summary>
        /// Gets or sets the identifier of the client requesting the token.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the application secret.
        /// </summary>
        /// <remarks>
        /// The application secret will only be presisted in Azure Key Vault. 
        /// </remarks>
        public string ApplicationSecret { get; set; }

        /// <summary>
        /// Gets or sets the Key Vault secret name for the secret of the client requesting the token.
        /// </summary>
        public string ApplicationSecretName { get; set; }

        /// <summary>
        /// Gets or sets the Azure AD tenant identifier.
        /// </summary>
        public string TenantId { get; set; }
    }
}