// -----------------------------------------------------------------------
// <copyright file="SyncCustomers.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Host;
    using Bindings;
    using Models;
    using Services;

    public static class SyncCustomers
    {
        [FunctionName("SyncCustomers")]
        [return: Customers(CollectionId = "Customers", DatabaseId = "SmartOffice")]
        public static async Task<List<Customer>> RunAsync(
            [TimerTrigger("0 0 12 * * *")]TimerInfo timerInfo,
            [Token(
                ApplicationId ="PartnerCenter.ApplicationId",
                SecretName = "PartnerCenterApplicationSecret",
                ApplicationTenantId = "PartnerCenter.AccountId",
                Resource = "https://graph.windows.net"
            )]string token,
            TraceWriter log)
        {
            List<Customer> customers;
            PartnerService partner;

            try
            {
                partner = new PartnerService("https://api.partnercenter.microsoft.com");

                customers = await partner.GetCustomersAsync(new RequestContext
                {
                    AccessToken = token,
                    CorrelationId = Guid.NewGuid(),
                    Locale = "en-US"
                }).ConfigureAwait(false);

                return customers;
            }
            finally
            {
                customers = null;
                partner = null;
            }
        }
    }
}