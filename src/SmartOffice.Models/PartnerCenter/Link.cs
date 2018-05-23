// -----------------------------------------------------------------------
// <copyright file="Link.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public sealed class Link
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Link" /> class.
        /// </summary>
        /// <param name="uri"></param>
        public Link(Uri uri)
          : this(uri, "GET", null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Link" /> class.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        [JsonConstructor]
        public Link(Uri uri, string method, IEnumerable<KeyValuePair<string, string>> headers = null)
        {
            Uri = uri;
            Method = method;
            Headers = headers ?? new List<KeyValuePair<string, string>>();
        }

        /// <summary>Gets the URI.</summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Gets the method.
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Gets the link headers.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Headers { get; private set; }
    }
}