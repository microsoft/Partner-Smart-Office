// -----------------------------------------------------------------------
// <copyright file="ProcessSubscriptionDetail.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models
{
    public sealed class ProcessSubscriptionDetail
    {
        /// <summary>
        /// Gets or sets the subscription details.
        /// </summary>
        public SubscriptionDetail Subscription { get; set; }

        /// <summary>
        /// Gets or sets the Partner Center endpoint.
        /// </summary>
        public EndpointDetail PartnerCenterEndpoint { get; set; }

    }
}