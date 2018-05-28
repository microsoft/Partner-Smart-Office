// -----------------------------------------------------------------------
// <copyright file="ICustomerOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Customers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter.Customers;
    using Subscriptions;

    public interface ICustomerOperations
    {
        Task<Customer> GetAsync(CancellationToken cancellationToken = default(CancellationToken));

        ISubscriptionCollectionOperations Subscriptions { get; }
    }
}