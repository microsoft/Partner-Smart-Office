// -----------------------------------------------------------------------
// <copyright file="ProcessCustomersTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Models;
    using Moq;
    using Services;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ProcessCustomersTests
    {
        [TestMethod]
        public async Task RunTestAsync()
        {
            Mock<IDocumentRepository<Customer>> repository;
            Mock<IStorageService> storage;
            TestTraceWriter traceWriter;

            try
            {
                repository = new Mock<IDocumentRepository<Customer>>();
                repository.Setup(r => r.GetAsync()).Returns(Task.FromResult(GetTestCustomers().AsEnumerable()));

                storage = new Mock<IStorageService>();
                storage.Setup(s => s.WriteToQueueAsync(It.IsAny<string>(), It.IsAny<Customer>())).Returns(Task.FromResult(0));

                traceWriter = new TestTraceWriter();

                await ProcessCustomers.RunAsync(
                    null,
                    repository.Object,
                    storage.Object,
                    traceWriter);

                Assert.AreEqual(2, traceWriter.TraceEvents.Count);
            }
            finally
            {
                repository = null;
                storage = null;
                traceWriter = null;
            }
        }

        private static List<Customer> GetTestCustomers()
        {
            return new List<Customer>
            {
                {
                    new Customer
                    {
                         CompanyProfile = new CompanyProfile
                         {
                             CompanyName = "Contoso",
                             Domain = "contoso.onmicrosoft.com"
                         },
                         Id = "54db2c9b-b904-4954-a347-637fa08b1d4c"
                    }
                },
                {
                    new Customer
                    {
                        CompanyProfile = new CompanyProfile
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