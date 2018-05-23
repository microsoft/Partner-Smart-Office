// -----------------------------------------------------------------------
// <copyright file="IPartnerServiceClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter
{
    using AuditRecords;
    using Customers;

    public interface IPartnerServiceClient
    {
        /// <summary>
        /// Gets the available audit record operations.
        /// </summary>
        IAuditRecordCollectionOperations AuditRecords { get; }

        /// <summary>
        /// Provides the available customer operations. 
        /// </summary>
        ICustomerCollectionOperations Customers { get; }
    }
}