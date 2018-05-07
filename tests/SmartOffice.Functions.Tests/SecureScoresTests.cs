// -----------------------------------------------------------------------
// <copyright file="SecureScoresTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Data;
    using Models;
    using Moq;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SecureScoresTests
    {
        [TestMethod]
        public async Task ImportControlsAsync()
        {
            Mock<IDocumentRepository<ControlListEntry>> repository;
            TestTraceWriter traceWriter;

            try
            {
                repository = new Mock<IDocumentRepository<ControlListEntry>>();
                repository.Setup(r => r.AddOrUpdateAsync(It.IsAny<IEnumerable<ControlListEntry>>())).Returns(Task.FromResult(0));

                traceWriter = new TestTraceWriter();

                await Security.ImportControlsAsync(
                    null,
                    repository.Object,
                    traceWriter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                repository = null;
                traceWriter = null;
            }
        }

        [TestMethod]
        public async Task ReceiveQueueTestAsync()
        {
            Customer customer;
            List<Alert> alerts;
            List<SecureScore> secureScore;
            Mock<IDocumentRepository<SecureScore>> scores;
            Mock<IDocumentRepository<Alert>> securityAlerts;
            TestTraceWriter traceWriter;

            try
            {
                customer = new Customer
                {
                    CompanyProfile = new CompanyProfile()
                    {
                        CompanyName = "Consoto",
                        Domain = "contoso.onmicrosoft.com"
                    },
                    Id = "e0fad1f4-c643-49f1-87b6-d3f1796e94b9"
                };

                alerts = new List<Alert>
                {
                    {
                        new Alert
                        {
                             AzureTenantId = "e0fad1f4-c643-49f1-87b6-d3f1796e94b9"
                        }
                    }
                };

                secureScore = new List<SecureScore>
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


                scores = new Mock<IDocumentRepository<SecureScore>>();
                scores.Setup(r => r.AddOrUpdateAsync(It.IsAny<List<SecureScore>>())).Returns(Task.FromResult(0));

                securityAlerts = new Mock<IDocumentRepository<Alert>>();
                securityAlerts.Setup(r => r.AddOrUpdateAsync(It.IsAny<List<Alert>>())).Returns(Task.FromResult(0));

                traceWriter = new TestTraceWriter();

                await Security.ProcessAsync(
                    customer,
                    scores.Object,
                    securityAlerts.Object,
                    secureScore,
                    alerts,
                    traceWriter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                customer = null;
                scores = null;
                secureScore = null;
                traceWriter = null;
            }
        }
    }
}