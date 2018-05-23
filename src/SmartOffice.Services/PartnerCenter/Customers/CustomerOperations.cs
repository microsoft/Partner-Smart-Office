// -----------------------------------------------------------------------
// <copyright file="CustomerOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter.Customers
{
    using System;
    using Subscriptions;

    public class CustomerOperations : ICustomerOperations
    {
        /// <summary>
        /// Provides the available subscription operations.
        /// </summary>
        private readonly Lazy<ISubscriptionCollectionOperations> subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerOperations" /> class.
        /// </summary>
        /// <param name="client">Provides the ability to perform HTTP operations.</param>
        /// <param name="customerId">Identifier for the customer.</param>
        public CustomerOperations(PartnerServiceClient client, string customerId)
        {
            subscriptions = new Lazy<ISubscriptionCollectionOperations>(() => new SubscriptionCollectionOperations(client, customerId));
        }

        /// <summary>
        /// Gets the available subscription operations.
        /// </summary>
        public ISubscriptionCollectionOperations Subscriptions => subscriptions.Value;
    }
}