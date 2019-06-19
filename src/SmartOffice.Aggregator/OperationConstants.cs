// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator
{
    /// <summary>
    /// Defines the constants used by the Azure Functions.
    /// </summary>
    public static class OperationConstants
    {
        /// <summary>
        /// Name of the account endpoint setting.
        /// </summary>
        public const string AccountEndpoint = "AccountEndpoint";

        /// <summary>
        /// Name of the account key setting.
        /// </summary>
        public const string AccountKey = "AccountKey";

        /// <summary>
        /// Name of the bulk import stored procedure.
        /// </summary>
        public const string BulkImportStoredProcedureName = "BulkImport";

        /// <summary>
        /// Name of the control profile sync operation.
        /// </summary>
        public const string ControlProfileSync = "sync-controlprofile";

        /// <summary>
        /// Name of the application setting that contains the Azure Cosmos DB connection string.
        /// </summary>
        public const string CosmosDbConnectionString = "CosmosDbConnectionString";

        /// <summary>
        /// Name of the database.
        /// </summary>
        public const string DatabaseId = "smartoffice";

        /// <summary>
        /// Name of the environments collection.
        /// </summary>
        public const string EnvironmentsCollectionId = "environments";

        /// <summary>
        /// Name of the partner delta sync storage queue.
        /// </summary>
        public const string PartnerDeltaSync = "sync-partnerdelta";

        /// <summary>
        /// Name of the partner full sync storage queue.
        /// </summary>
        public const string PartnerFullSync = "sync-partnerfull";

        /// <summary>
        /// Name of the collection where security events will be stored.
        /// </summary>
        public const string SecurityEventsCollectionId = "securityevents";

        /// <summary>
        /// Name of the security event sync operation.
        /// </summary>
        public const string SecurityEventSync = "sync-securityevent";

        /// <summary>
        /// Name of the start sync operation.
        /// </summary>
        public const string StartSync = "sync-start";

        /// <summary>
        /// Name of the application setting that contains the storage connection string.
        /// </summary>
        public const string StorageConnectionString = "StorageConnectionString";

        /// <summary>
        /// Name of the subscriptions sync operation.
        /// </summary>
        public const string SubscriptionSync = "sync-subscriptions";
    }
}