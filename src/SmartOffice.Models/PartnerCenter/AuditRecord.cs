// -----------------------------------------------------------------------
// <copyright file="AuditRecord.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json; 

    public class AuditRecord
    {
        public string ApplicationId { get; set; }

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }

        public IEnumerable<KeyValuePair<string, string>> CustomizedData { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public DateTime OperationDate { get; set; }

        public OperationStatus OperationStatus { get; set; }

        public OperationType OperationType { get; set; }

        public string OriginalCorrelationId { get; set; }

        public string PartnerId { get; set; }

        public string ResourceOldValue { get; set; }

        public string ResourceNewValue { get; set; }

        public ResourceType ResourceType { get; set; }

        public string UserPrincipalName { get; set; }
    }
}