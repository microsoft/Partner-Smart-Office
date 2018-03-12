// -----------------------------------------------------------------------
// <copyright file="PartnerServiceTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------


namespace Microsoft.Partner.SmartOffice.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PartnerServiceTests
    {
        private const string PartnerCenterEndpoint = "https://api.partnercenter.microsoft.com";

        [TestMethod]
        public async Task GetCustomersTestAsync()
        {
            IPartnerService service;
            List<Customer> customers;

            try
            {
                service = new PartnerService(new TestHttpService(), PartnerCenterEndpoint);

                customers = await service.GetCustomersAsync(new RequestContext
                {
                    AccessToken = string.Empty,
                    CorrelationId = Guid.NewGuid(),
                    Locale = "en-US"
                }).ConfigureAwait(false);

                Assert.IsNotNull(customers);
                Assert.AreEqual(customers.Count, 1);
            }
            finally
            {
                customers = null;
                service = null;
            }
        }
    }
}