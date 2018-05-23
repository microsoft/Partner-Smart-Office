// -----------------------------------------------------------------------
// <copyright file="NetworkConnection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    using System.Collections.Generic; 

    public class NetworkConnection
    {
        public string DestinationAddress { get; set; }

        public string DestinationPort { get; set; }

        public SecurityNetworkProtocol? Protocol { get; set; }

        public string SourceAddress { get; set; }

        public string SourcePort { get; set; }

        public string Uri { get; set; }

        public IDictionary<string, object> AdditionalData { get; set; }
    }
}
