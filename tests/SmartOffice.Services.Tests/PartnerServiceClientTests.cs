// -----------------------------------------------------------------------
// <copyright file="PartnerServiceTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Tests
{
    using System;
    using System.Threading.Tasks;
    using Azure.Test.HttpRecorder;
    using Models.PartnerCenter;
    using Models.PartnerCenter.Customers;
    using Rest.ClientRuntime.Azure.TestFramework;
    using Services.PartnerCenter;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PartnerServiceClientTests
    {
        /// <summary>
        /// The endpoint address for the Partner Center service.
        /// </summary>
        private const string PartnerCenterEndpoint = "https://api.partnercenter.microsoft.com";

        [TestMethod]
        public async Task GetCustomersTestAsync()
        {
            IPartnerServiceClient partnerService; 
            SeekBasedResourceCollection<Customer> customers;

            try
            {
                using (MockContext context = MockContext.Start(GetType().Name))
                {
                    HttpMockServer.Initialize(GetType().Name, "GetCustomersTestAsync", HttpRecorderMode.Playback);

                    partnerService = new PartnerServiceClient(
                        new Uri(PartnerCenterEndpoint),
                        new TestServiceCredentials("STUB_TOKEN"),
                        HttpMockServer.CreateInstance());

                    customers = await partnerService.Customers.GetAsync().ConfigureAwait(false);

                    Assert.IsNotNull(customers);

                    context.Stop();
                }
            }
            finally
            {
                customers = null;
                partnerService = null;
            }
        }
    }
}