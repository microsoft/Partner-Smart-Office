// -----------------------------------------------------------------------
// <copyright file="AuditRecord.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.AuditRecords
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class AuditRecord
    {
        /// <summary>
        /// Gets or sets the identifier of the application that invoked the operation.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of customer where the operation was performed.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer where the operation was performed.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the dictionary that contains additional data that customized to the operation performed.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> CustomizedData { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the audit record.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the date when the operation that was performed.
        /// </summary>
        public DateTime OperationDate { get; set; }

        /// <summary>
        /// Gets or sets the status for the operation that was performed.
        /// </summary>
        public OperationStatus OperationStatus { get; set; }

        /// <summary>
        /// Gets or sets the type for the operation that was performed.
        /// </summary>
        public OperationType OperationType { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier for the initial operation that created the audit record.
        /// </summary>
        public string OriginalCorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the partner.
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the old value of the resource.
        /// </summary>
        public string ResourceOldValue { get; set; }

        /// <summary>
        /// Gets or sets the new value of the resource.
        /// </summary>
        public string ResourceNewValue { get; set; }

        /// <summary>
        /// Gets or sets the type of resource acted upon by the operation. 
        /// </summary>
        public ResourceType ResourceType { get; set; }

        /// <summary>
        /// Gets or sets the user principal name of the user who performed the operation.
        /// </summary>
        public string UserPrincipalName { get; set; }
    }
}