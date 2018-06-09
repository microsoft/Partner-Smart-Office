// -----------------------------------------------------------------------
// <copyright file="IDocumentRepository.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IDocumentRepository<TEntity>
    {
        /// <summary>
        /// Add or update an item in the repository.
        /// </summary>
        /// <param name="item">The item to be added or updated.</param>
        /// <param name="partitionKey">Key used to partition the data.</param>
        /// <returns>The entity that was added or updated.</returns>
        Task<TEntity> AddOrUpdateAsync(TEntity item, string partitionKey = null);

        /// <summary>
        /// Add or update a collection of items in the repository.
        /// </summary>
        /// <param name="items">A collection of items to be added or updated.</param>
        /// <param name="partitionKey">Key used to partition the data.</param>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        Task AddOrUpdateAsync(IEnumerable<TEntity> items, string partitionKey = null);

        /// <summary>
        /// Deletes the document associated with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier of the document.</param>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// Gets an item from the repository.
        /// </summary>
        /// <param name="id">Identifier of the item.</param>
        /// <returns>
        /// The item that matches the specified identifier; if not found null.
        /// </returns>
        Task<TEntity> GetAsync(string id);

        /// <summary>
        /// Gets all items available in the repository.
        /// </summary>
        /// <returns>
        /// A collection of items that represent the items in the repository.
        /// </returns>
        Task<List<TEntity>> GetAsync();

        /// <summary>
        /// Gets a sequence of items for the repository that matches the query. 
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="partitionKey">Key used to partition the data.</param>
        /// <returns>
        /// A collection that contains items from the repository that satisfy the condition specified by predicate.
        /// </returns>
        Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, string partitionKey);

        /// <summary>
        /// Performs the initialization operations for the repository.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        Task InitializeAsync();

        /// <summary>
        /// Updates an item in the repository.
        /// </summary>
        /// <param name="item">The item to be updated.</param>
        /// <returns>The entity that updated.</returns>
        Task<TEntity> UpdateAsync(TEntity item);
    }
}