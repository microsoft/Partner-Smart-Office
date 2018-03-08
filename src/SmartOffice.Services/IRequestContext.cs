// -----------------------------------------------------------------------
// <copyright file="IRequestContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services
{
    using System;

    /// <summary>
    /// Context information amended to HTTP operations.
    /// </summary>
    public interface IRequestContext
    {
        /// <summary>
        /// Gets or sets the acess token.
        /// </summary>
        string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets name of the application.
        /// </summary>
        string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier that is used to group operations together.
        /// </summary>
        Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the locale. 
        /// </summary>
        string Locale { get; set; }

        /// <summary>
        /// Gets the request identifier used to uniquely identify operations.
        /// </summary>
        Guid RequestId { get; }
    }
}