// -----------------------------------------------------------------------
// <copyright file="ODataResponse.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ODataResponse<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ODataResponse{T}" /> class.
        /// </summary>
        public ODataResponse()
        {
            Value = new List<T>();
        }

        /// <summary>
        /// Gets or sets the value for the response.
        /// </summary>
        [JsonProperty("value")]
        public List<T> Value { get; }

        /// <summary>
        /// Gets or sets the additional data for the response.
        /// </summary>
        [JsonExtensionData(ReadData = true)]
        public IDictionary<string, object> AdditionalData { get; set; }
    }
}