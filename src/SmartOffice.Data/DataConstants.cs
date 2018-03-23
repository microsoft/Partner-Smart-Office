// -----------------------------------------------------------------------
// <copyright file="PartnerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Data
{
    public sealed class DataConstants
    {
        /// <summary>
        /// Identifier for the bulk import stored procedure.
        /// </summary>
        public const string BulkImportStoredProcedureId = "BulkImport";

        /// <summary>
        /// Identifier for the customers collection.
        /// </summary>
        public const string CustomersCollectionId = "Customers";

        /// <summary>
        /// Identifier for the Azure Cosmos DB database.
        /// </summary>
        public const string DatabaseId = "SmartOffice";

        /// <summary>
        /// Identifier for the secure score collection.
        /// </summary>
        public const string SecureScoreCollectionId = "SecureScore";
    }
}