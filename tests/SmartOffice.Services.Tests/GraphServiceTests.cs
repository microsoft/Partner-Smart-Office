// -----------------------------------------------------------------------
// <copyright file="GraphServiceTests.cs" company="Microsoft">
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
    public class GraphServiceTests
    {
        /// <summary>
        /// Endpoint address for the Microsoft Graph service.
        /// </summary>
        private const string GraphEndpoint = "https://graph.microsoft.com";

        [TestMethod]
        public async Task GetSecureScoreTestAsync()
        {
            IGraphService service;
            List<SecureScore> secureScore;

            try
            {
                service = new GraphService(new TestHttpService(), GraphEndpoint);

                secureScore = await service.GetSecureScoreAsync(new RequestContext
                {
                    AccessToken = string.Empty,
                    CorrelationId = Guid.NewGuid(),
                    Locale = "en-US"
                },
                1).ConfigureAwait(false);

                Assert.IsNotNull(secureScore);
                Assert.AreEqual(secureScore.Count, 1);
            }
            finally
            {
                secureScore = null;
                service = null;
            }
        }
    }
}