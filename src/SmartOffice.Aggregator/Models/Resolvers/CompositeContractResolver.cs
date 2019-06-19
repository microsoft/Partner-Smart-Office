// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Models.Resolvers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Provides the ability to have multiple contract resolvers.
    /// </summary>
    public class CompositeContractResolver : IContractResolver, IEnumerable<IContractResolver>
    {
        /// <summary>
        /// The list of configured contract resolvers.
        /// </summary>
        private readonly List<IContractResolver> resolvers;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeContractResolver" /> class.
        /// </summary>
        public CompositeContractResolver()
        {
            resolvers = new List<IContractResolver>();
        }

        /// <summary>
        /// Gets an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IContractResolver> GetEnumerator()
        {
            return resolvers.GetEnumerator();
        }

        /// <summary>
        /// Resolves the contract for a given type.
        /// </summary>
        /// <param name="type">The type for which a contract should be resolved.</param>
        /// <returns>The contract for a given type.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is null.
        /// </exception>
        public JsonContract ResolveContract(Type type)
        {
            type.AssertNotNull(nameof(type));

            return resolvers.Select(r => r.ResolveContract(type)).FirstOrDefault();
        }

        /// <summary>
        /// Adds a contract resolver to the list of resolvers.
        /// </summary>
        /// <param name="resolver">The resolver to be added to the list of resolvers.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resolver"/> is null.
        /// </exception>
        public void Add(IContractResolver resolver)
        {
            resolver.AssertNotNull(nameof(resolver));

            resolvers.Add(resolver);
        }

        /// <summary>
        /// Gets an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}