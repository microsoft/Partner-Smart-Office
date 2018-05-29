// -----------------------------------------------------------------------
// <copyright file="CreatedDate.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Graph
{
    /// <summary>
    /// Represents the point in time when an instance of <see cref="SecureScore"/> was created.
    /// </summary>
    public class CreatedDate
    {
        /// <summary>
        /// Gets or sets the day.
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        public int Year { get; set; }
    }
}