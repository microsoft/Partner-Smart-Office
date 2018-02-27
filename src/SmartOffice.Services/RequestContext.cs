// -----------------------------------------------------------------------
// <copyright file="RequestContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;

    public class RequestContext : IRequestContext
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RequestContext"/> class.
        /// </summary>
        public RequestContext() : this(Guid.NewGuid(), string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RequestContext"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation identifier that is used to group operations together.</param>
        public RequestContext(Guid correlationId) : this(correlationId, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="RequestContext"/> class.
        /// </summary>
        /// <param name="correlationId">The correlation identifier that is used to group operations together.</param>
        /// <param name="locale">The locale for the request.</param>
        public RequestContext(Guid correlationId, string locale)
        {
            CorrelationId = correlationId;
            Locale = locale;
        }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier that is used to group operations together.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the locale. 
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Gets the request identifier used to uniquely identify operations.
        /// </summary>
        public Guid RequestId => Guid.NewGuid();
    }
}