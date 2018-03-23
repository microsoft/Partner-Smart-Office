// -----------------------------------------------------------------------
// <copyright file="PartnerServiceTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Azure.Test.HttpRecorder;
    using Models;
    using Rest.ClientRuntime.Azure.TestFramework;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PartnerServiceTests
    {
        /// <summary>
        /// The endpoint address for the Partner Center service.
        /// </summary>
        private const string PartnerCenterEndpoint = "https://api.partnercenter.microsoft.com";

        [TestMethod]
        public async Task GetCustomersTestAsync()
        {
            IPartnerService partner; 
            List<Customer> customers;

            try
            {
                using (MockContext context = MockContext.Start(GetType().Name))
                {
                    HttpMockServer.Initialize(GetType().Name, "GetCustomersTestAsync", HttpRecorderMode.Playback);

                    var name = Path.Combine(HttpMockServer.RecordsDirectory, HttpMockServer.CallerIdentity);

                    partner = new PartnerService(
                        new Uri(PartnerCenterEndpoint),
                        new TestServiceCredentials("STUB_TOKEN"),
                        HttpMockServer.CreateInstance());

                    customers = await partner.GetCustomersAsync().ConfigureAwait(false);

                    Assert.IsNotNull(customers);

                    context.Stop();
                }
            }
            finally
            {
                customers = null;
                partner = null;
            }
        }
    }
}