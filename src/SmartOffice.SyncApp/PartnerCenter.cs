// -----------------------------------------------------------------------
// <copyright file="PartnerCenter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------


namespace Microsoft.Partner.SmartOffice.SyncApp
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Store.PartnerCenter;
    using Store.PartnerCenter.Enumerators;
    using Store.PartnerCenter.Extensions;
    using Store.PartnerCenter.Models;
    using Store.PartnerCenter.RequestContext;

    public class PartnerCenter
    {
        /// <summary>
        /// Name of the application communicating with Partner Center.
        /// </summary>
        private const string ApplicationName = "Partner-Smart-Office SyncApp";

        /// <summary>
        /// Gets a list of customers from Partner Center.
        /// </summary>
        /// <param name="correlationId">Correlation identifier used when communicating with Partner Center</param>
        /// <returns>A list of customers from Partner Center.</returns>
        public async Task<List<Customer>> GetCustomersAsync(Guid correlationId = default(Guid))
        {
            IPartner operations;
            IResourceCollectionEnumerator<SeekBasedResourceCollection<Store.PartnerCenter.Models.Customers.Customer>> customersEnumerator;
            List<Customer> customers;
            SeekBasedResourceCollection<Store.PartnerCenter.Models.Customers.Customer> seekCustomers;

            try
            {
                if (correlationId == default(Guid))
                {
                    correlationId = Guid.NewGuid();
                }

                customers = new List<Customer>();
                operations = await GetPartnerServiceAsync(correlationId).ConfigureAwait(false);
                seekCustomers = await operations.Customers.GetAsync().ConfigureAwait(false);

                customersEnumerator = operations.Enumerators.Customers.Create(seekCustomers);

                while (customersEnumerator.HasValue)
                {
                    customers.AddRange(customersEnumerator.Current.Items.Select(c => new Customer
                    {
                        CompanyName = c.CompanyProfile.CompanyName,
                        Id = c.Id
                    }));

                    await customersEnumerator.NextAsync().ConfigureAwait(false);
                }

                return customers;
            }
            finally
            {
                operations = null;
            }
        }

        /// <summary>
        /// Gets an aptly configured instance of the partner service. 
        /// </summary>
        /// <param name="correlationId">Correlation identifier used when communicating with Partner Center</param>
        /// <returns>An aptly populated instance of the partner service.</returns>
        /// <remarks>
        /// This function will request the necessary access token to communicate with Partner Center and initialize 
        /// an instance of the partner service. The application name and correlation identifier are optional values, however, 
        /// they have been included here because it is considered best practice. Including the application name makes it where
        /// Microsoft can quickly identify what application is communicating with Partner Center. Specifying the correlation 
        /// identifier should be done to easily correlate a series of calls to Partner Center. Both of these properties will 
        /// help Microsoft with identifying issues and supporting you. 
        /// </remarks>
        private async Task<IPartner> GetPartnerServiceAsync(Guid correlationId)
        {
            IPartnerCredentials credentials = await PartnerCredentials.Instance.GenerateByApplicationCredentialsAsync(
                ConfigurationManager.AppSettings["PartnerCenter.ApplicationId"],
                ConfigurationManager.AppSettings["PartnerCenter.ApplicationSecret"],
                ConfigurationManager.AppSettings["PartnerCenter.AccountId"]).ConfigureAwait(false);

            IAggregatePartner partner = PartnerService.Instance.CreatePartnerOperations(credentials);

            PartnerService.Instance.ApplicationName = ApplicationName;

            return partner.With(RequestContextFactory.Instance.Create(correlationId));
        }
    }
}