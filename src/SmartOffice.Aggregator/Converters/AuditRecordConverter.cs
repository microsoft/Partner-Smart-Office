// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Converters
{
    using Microsoft.Store.PartnerCenter.Models.Auditing;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides the ability to convert an instance of <see cref="AuditRecord" /> to the defined type.
    /// </summary>
    public static class AuditRecordConverter
    {
        /// <summary>
        /// Converts an instance of <see cref="AuditRecord" /> to the specified type.
        /// </summary>
        /// <typeparam name="TOutput">Type of object to be returned.</typeparam>
        /// <param name="record">An audit record from Partner Center.</param>
        /// <returns>An entity that represents the modified resource.</returns>
        public static TOutput Convert<TOutput>(AuditRecord record)
        {
            return JsonConvert.DeserializeObject<TOutput>(record.ResourceNewValue);
        }
    }
}