// -----------------------------------------------------------------------
// <copyright file="EnvironmentsTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.WebJobs;
    using Azure.WebJobs.Extensions.Timers;
    using Data;
    using Models;
    using Models.PartnerCenter.Customers;
    using Moq;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EnvironmentsTests
    {
        [TestMethod]
        public async Task PullEnvironmentsAsync()
        {
            Mock<IDocumentRepository<EnvironmentDetail>> repository;
            TestTraceWriter traceWriter;
            TimerInfo timerInfo;

            try
            {
                repository = new Mock<IDocumentRepository<EnvironmentDetail>>();
                repository.Setup(
                    r => r.GetAsync()).Returns(Task.FromResult(GetTestEnvironments()));

                //storage = new Mock<IStorageService>();
                //storage.Setup(s => s.WriteToQueueAsync(
                //    It.IsAny<string>(), It.IsAny<EnvironmentDetail>())).Returns(Task.FromResult(0));

                timerInfo = new TimerInfo(
                    new TestTimerSchedule(),
                    new ScheduleStatus(),
                    false);

                traceWriter = new TestTraceWriter();

                //await Environments.PullEnvironmentsAsync(
                //    timerInfo,
                //    repository.Object,
                //    storage.Object,
                //    traceWriter).ConfigureAwait(false);

                await Task.CompletedTask.ConfigureAwait(false);
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

        private static List<EnvironmentDetail> GetTestEnvironments()
        {
            return new List<EnvironmentDetail>
            {
                {
                    new EnvironmentDetail
                    {
                        AppEndpoint = new EndpointDetail
                        {
                            ApplicationId = Guid.NewGuid().ToString(),
                            ApplicationSecretId = Guid.NewGuid().ToString(),
                            ServiceAddress = "https://graph.microsoft.com",
                            TenantId = Guid.NewGuid().ToString()
                        },
                        EnvironmentType = EnvironmentType.CSP,
                        FriendlyName = "Test CSP Environment",
                        Id = Guid.NewGuid().ToString(),
                        LastProcessed = DateTimeOffset.UtcNow.AddDays(-1),
                        PartnerCenterEndpoint = new EndpointDetail
                        {
                            ApplicationId = Guid.NewGuid().ToString(),
                            ApplicationSecretId = Guid.NewGuid().ToString(),
                            ServiceAddress = "https://api.partnercenter.microsoft.com",
                            TenantId = Guid.NewGuid().ToString()
                        },
                        ProcessAzureUsage = true
                    }
                },
                {
                    new EnvironmentDetail
                    {
                        AppEndpoint = new EndpointDetail
                        {
                            ApplicationId = Guid.NewGuid().ToString(),
                            ApplicationSecretId = Guid.NewGuid().ToString(),
                            ServiceAddress = "https://graph.microsoft.com",
                            TenantId = Guid.NewGuid().ToString()
                        },
                        EnvironmentType = EnvironmentType.EA,
                        FriendlyName = "Test EA Environment",
                        Id = Guid.NewGuid().ToString(),
                        LastProcessed = DateTimeOffset.UtcNow.AddDays(-1),
                        ProcessAzureUsage = true
                    }
                }
            };
        }

        private static List<CustomerDetail> GetTestCustomers()
        {
            return new List<CustomerDetail>
            {
                {
                    new CustomerDetail
                    {
                         CompanyProfile = new CustomerCompanyProfile
                         {
                             CompanyName = "Contoso",
                             Domain = "contoso.onmicrosoft.com"
                         },
                         Id = "54db2c9b-b904-4954-a347-637fa08b1d4c"
                    }
                },
                {
                    new CustomerDetail
                    {
                        CompanyProfile = new CustomerCompanyProfile
                        {
                            CompanyName = "Fabrikam",
                            Domain = "fabrikam.onmicrosoft.com"
                        },
                        Id = "f03c2609-15ed-4732-bd28-4d5369eeebd4"
                    }
                }
            };
        }
    }
}