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
        public async Task ReceiveQueueTestAsync()
        {
            Customer customer;
            List<SecureScore> secureScore;
            Mock<IDocumentRepository<SecureScore>> repository;
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

                repository = new Mock<IDocumentRepository<SecureScore>>();

                repository.Setup(r => r.AddOrUpdateAsync(It.IsAny<List<SecureScore>>())).Returns(Task.FromResult(0));

                traceWriter = new TestTraceWriter();

                await SecureScores.ProcessAsync(
                    customer,
                    repository.Object,
                    secureScore,
                    traceWriter).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                customer = null;
                repository = null;
                secureScore = null;
                traceWriter = null;
            }
        }
    }
}