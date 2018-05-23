// -----------------------------------------------------------------------
// <copyright file="ICustomerOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Customers
{
    using Subscriptions;

    public interface ICustomerOperations
    {
        ISubscriptionCollectionOperations Subscriptions { get; }
    }
}