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
        /// <summary>
        /// Gets the information for the customer.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The information for the customer.</returns>
        Task<Customer> GetAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Obtains the subscriptions behavior for the customer.
        /// </summary>
        ISubscriptionCollectionOperations Subscriptions { get; }
    }
}