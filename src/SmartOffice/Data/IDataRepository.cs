// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDataRepository<TEntry>
    {
        /// <summary>
        /// Deletes the document associated with the specified identifier.
        /// </summary>
        /// <param name="id">Identifier of the document.</param>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        Task DeleteAsync(string id);

        /// <summary>
        /// Gets all items available in the repository.
        /// </summary>
        /// <returns>
        /// A collection of items that represent the items in the repository.
        /// </returns>
        Task<List<TEntry>> GetAsync();

        /// <summary>
        /// Gets an item from the repository.
        /// </summary>
        /// <param name="id">Identifier of the item.</param>
        /// <returns>
        /// The item that matches the specified identifier; if not found null.
        /// </returns>
        Task<TEntry> GetAsync(string id);

        /// <summary>
        /// Updates an item in the repository.
        /// </summary>
        /// <param name="item">The item to be updated.</param>
        /// <returns>The entry that was updated.</returns>
        Task<TEntry> UpdateAsync(TEntry item);
    }
}