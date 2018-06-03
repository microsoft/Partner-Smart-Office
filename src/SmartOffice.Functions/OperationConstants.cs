// -----------------------------------------------------------------------
// <copyright file="OperationConstants.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions
{
    /// <summary>
    /// Provides common constants related to operations performed.
    /// </summary>
    internal static class OperationConstants
    {
        /// <summary>
        /// Name of the customers storage queue.
        /// </summary>
        public const string CustomersQueueName = "customers";

        /// <summary>
        /// Name of ther partners storage queue.
        /// </summary>
        public const string PartnersQueueName = "partners";

        /// <summary>
        /// Name of the security storage queue.
        /// </summary>
        public const string SecurityQueueName = "security";

        /// <summary>
        /// Name of the utilization storage queue.
        /// </summary>
        public const string UtilizationQueueName = "utilization";
    }
}