// -----------------------------------------------------------------------
// <copyright file="EnvironmentsTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Models;
    using Models.Graph;
    using Moq;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EnvironmentsTests
    {
        [TestMethod]
        public async Task ProcessCustomerAsync()
        {
            CustomerDetail customer;
            List<Alert> alerts;
            List<SecureScore> scores;
            Mock<IDocumentRepository<Alert>> alertsRepository;
            Mock<IDocumentRepository<SecureScore>> secureScoreRepository;
            TestTraceWriter traceWriter;

            try
            {
                alerts = new List<Alert>
                {
                    {
                        new Alert
                        {
                             AzureTenantId = "e0fad1f4-c643-49f1-87b6-d3f1796e94b9"
                        }
                    }
                };

                alertsRepository = new Mock<IDocumentRepository<Alert>>();
                alertsRepository.Setup(r => r.AddOrUpdateAsync(It.IsAny<IEnumerable<Alert>>())).Returns(Task.FromResult(0));

                customer = new CustomerDetail
                {
                    AppEndpoint = new EndpointDetail
                    {
                        /* Omitted because this information is not used by this test. */
                    },
                    Id = "e0fad1f4-c643-49f1-87b6-d3f1796e94b9"
                };

                scores = new List<SecureScore>
                {
                    {
                        new SecureScore
                        {
                            AccountScore = 10,
                            ActiveUserCount = 10,
                            AverageAccountScore = 10,
                            AverageDataScore = 10,
                            AverageDeviceScore = 10,
                            AverageMaxSecureScore = 10,
                            AverageSecureScore = 10,
                            TenantId = "e0fad1f4-c643-49f1-87b6-d3f1796e94b9"
                        }
                    }
                };

                secureScoreRepository = new Mock<IDocumentRepository<SecureScore>>();
                secureScoreRepository.Setup(r => r.AddOrUpdateAsync(It.IsAny<IEnumerable<SecureScore>>())).Returns(Task.FromResult(0));

                traceWriter = new TestTraceWriter();

                await Environments.ProcessCustomerAsync(
                    customer,
                    secureScoreRepository.Object,
                    alertsRepository.Object,
                    scores,
                    alerts,
                    traceWriter).ConfigureAwait(false);

                // Verify that all verifable exceptation have been met.
                alertsRepository.Verify();
                secureScoreRepository.Verify();

                /*
                 * Verify that four log entries have been written. This indicates
                 * that all operations were successfully performed.
                 */ 
                Assert.AreEqual(traceWriter.TraceEvents.Count, 4);
            }
            finally
            {
                alertsRepository = null;
                secureScoreRepository = null;
                traceWriter = null;
            }
        }
    }
}