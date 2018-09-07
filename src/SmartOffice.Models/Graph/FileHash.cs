// -----------------------------------------------------------------------
// <copyright file="FileHash.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System.Collections.Generic;
    using Newtonsoft.Json; 

   public class FileHash
    {
        [JsonProperty]
        public FileHashType? HashType { get; set; }

        [JsonProperty]
        public string HashValue { get; set; }

        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}
