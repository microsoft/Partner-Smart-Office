// -----------------------------------------------------------------------
// <copyright file="ICustomerCollectionOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Customers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Models.PartnerCenter;
    using Models.PartnerCenter.Customers;

    public interface ICustomerCollectionOperations
    {
        /// <summary>
        /// Gets the customer operations for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of the <see cref="CustomerOperations" /> class.</returns>
        ICustomerOperations this[string customerId] { get; }

        /// <summary>
        /// Gets the customer operations for the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of the <see cref="CustomerOperations" /> class.</returns>
        ICustomerOperations ById(string customerId);

        /// <summary>
        /// Gets a collection of customers associated with the partner.
        /// </summary>
        /// <param name="nextLink">An instance of the <see cref="Link" /> class that represents the next page of records to be requested.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A collection of customers associated with the partner.</returns>
        Task<SeekBasedResourceCollection<Customer>> GetAsync(Link nextLink = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}