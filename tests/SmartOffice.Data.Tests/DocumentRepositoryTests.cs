// -----------------------------------------------------------------------
// <copyright file="DocumentRepositoryTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Azure.Documents;
    using Azure.Documents.Client;
    using Models;
    using Models.PartnerCenter; 
    using Moq;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DocumentRepositoryTests
    {
        private const string CustomerCollectionId = "Customers";

        private const string DatabaseId = "SmartOffice";

        [TestMethod]
        public async Task AddorUpdateTestAsync()
        {
            Customer customer;
            Customer excepted;
            IDocumentRepository<Customer> customers;
            Mock<IDocumentClient> client;

            try
            {
                client = new Mock<IDocumentClient>();

                excepted = GetTestCustomer();

                client.Setup(c => c.UpsertDocumentAsync(
                    It.IsAny<Uri>(), 
                    It.IsAny<object>(), 
                    It.IsAny<RequestOptions>(), 
                    It.IsAny<bool>())).Returns(Task.FromResult(new ResourceResponse<Document>(GetDocument(excepted))));

                customers = new DocumentRepository<Customer>(client.Object, DatabaseId, CustomerCollectionId);

                customer = await customers.AddOrUpdateAsync(excepted).ConfigureAwait(false);

                Assert.AreEqual(customer.CompanyProfile.CompanyName, excepted.CompanyProfile.CompanyName);
                Assert.AreEqual(customer.CompanyProfile.Domain, excepted.CompanyProfile.Domain);
                Assert.AreEqual(customer.Id, excepted.Id);
            }
            finally
            {
                client = null;
                customer = null;
                customers = null;
                excepted = null;
            }
        }

        [TestMethod]
        public async Task GetTestAsync()
        {
            Customer customer;
            Customer excepted;
            IDocumentRepository<Customer> repository;
            Mock<IDocumentClient> client;

            try
            {
                client = new Mock<IDocumentClient>();

                excepted = GetTestCustomer();

                client.Setup(c => c.ReadDocumentAsync(It.IsAny<Uri>(), It.IsAny<RequestOptions>())).Returns(
                    Task.FromResult(new ResourceResponse<Document>(GetDocument(excepted))));

                repository = new DocumentRepository<Customer>(client.Object, DatabaseId, CustomerCollectionId);

                customer = await repository.GetAsync("54db2c9b-b904-4954-a347-637fa08b1d4c").ConfigureAwait(false);

                Assert.AreEqual(customer.CompanyProfile.CompanyName, excepted.CompanyProfile.CompanyName);
                Assert.AreEqual(customer.CompanyProfile.Domain, excepted.CompanyProfile.Domain);
                Assert.AreEqual(customer.Id, excepted.Id);
            }
            finally
            {
                client = null;
                repository = null;
            }
        }

        [TestMethod]
        public async Task GetItemsTestAsync()
        {
            IEnumerable<Customer> customers;
            IDocumentRepository<Customer> repository;
            Mock<IDocumentClient> client;

            try
            {
                client = new Mock<IDocumentClient>();
                client.Setup(c => c.ReadDocumentFeedAsync(It.IsAny<Uri>(), It.IsAny<FeedOptions>())).Returns(
                    Task.FromResult(new FeedResponse<dynamic>(GetTestCustomers())));

                repository = new DocumentRepository<Customer>(client.Object, DatabaseId, CustomerCollectionId);

                customers = await repository.GetAsync().ConfigureAwait(false);

                Assert.AreEqual(2, customers.Count());
            }
            finally
            {
                client = null;
                customers = null;
                repository = null;
            }
        }

        private static Customer GetTestCustomer()
        {
            return new Customer
            {
                CompanyProfile = new CompanyProfile()
                {
                    CompanyName = "Consoto",
                    Domain = "contoso.onmicrosoft.com"
                },
                Id = "54db2c9b-b904-4954-a347-637fa08b1d4c"
            };
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

        private static Document GetDocument<TEntity>(TEntity entity)
        {
            Document document = new Document();

            foreach (PropertyInfo property in entity.GetType().GetProperties())
            {
                document.SetPropertyValue(property.Name, property.GetValue(entity));
            }

            return document;
        }
    }
}