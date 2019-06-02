// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    /// <summary>
    /// Defines the constants used by the Azure Functions.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Name of the bulk import stored procedure.
        /// </summary>
        public const string BulkImportStoredProcedureName = "BulkImport";

        /// <summary>
        /// Name of the change feed environments operation.
        /// </summary>
        public const string ChangeFeedEnvironments = "changeFeed-environments";

        /// <summary>
        /// Name of the control profile sync operation.
        /// </summary>
        public const string ControlProfileSync = "sync-controlprofile";

        /// <summary>
        /// Name of the application setting that contains the Azure Cosmos DB connection string.
        /// </summary>
        public const string CosmosDbConnectionString = "CosmosDbConnectionString";

        /// <summary>
        /// Name of the database where entries will be stored.
        /// </summary>
        public const string DatabaseName = "smartoffice";

        /// <summary>
        /// Name of the collection where environment information will be stored.
        /// </summary>
        public const string EnvironmentsCollection = "environments";

        /// <summary>
        /// Name of the collection where lease information will be stored.
        /// </summary>
        public const string LeasesCollection = "leases";

        /// <summary>
        /// Name of the partner delta sync operation.
        /// </summary>
        public const string PartnerDeltaSync = "sync-partnerdelta";

        /// <summary>
        /// Name of the partner full sync operation.
        /// </summary>
        public const string PartnerFullSync = "sync-partnerfull";

        /// <summary>
        /// Name of the collection where security events will be stored.
        /// </summary>
        public const string SecurityEventsCollection = "securityevents";

        /// <summary>
        /// Name of the start sync operation.
        /// </summary>
        public const string StartSync = "sync-start";

        /// <summary>
        /// Name of the application setting that contains the storage connection string.
        /// </summary>
        public const string StorageConnectionString = "StorageConnectionString";

        /// <summary>
        /// Name of the security event sync operation.
        /// </summary>
        public const string SecurtityEventSync = "sync-securityevent";
    }
}