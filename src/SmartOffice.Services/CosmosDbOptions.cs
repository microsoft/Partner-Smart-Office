// -----------------------------------------------------------------------
// <copyright file="CosmosDbOptions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System.Collections.Generic;

    public class CosmosDbOptions
    {
        /// <summary>
        /// Gets or sets the secret name for the Azure Cosmos DB access key.
        /// </summary>
        public string AccessKeySecretName { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the bulk import stored procedure.
        /// </summary>
        public string BulkImportStoredProcedureId { get; set; }

        /// <summary>
        /// Gets or sets a list of collections.
        /// </summary>
        public List<string> Collections { get; set; }

        /// <summary>
        /// Gets or sets the database identifier.
        /// </summary>
        public string DatabaseId { get; set; }

        /// <summary>
        /// Gets or sets the Azure Cosmos DB endpoint.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the Azure Key Vault endpoint.
        /// </summary>
        public string KeyVaultEndpoint { get; set; }
    }
}