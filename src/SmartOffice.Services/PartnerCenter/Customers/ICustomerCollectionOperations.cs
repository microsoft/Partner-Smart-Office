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

        Task<SeekBasedResourceCollection<Customer>> GetAsync(Link nextLink = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}