// -----------------------------------------------------------------------
// <copyright file="GraphServiceTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Test.HttpRecorder;
    using Models.Graph;
    using Rest.ClientRuntime.Azure.TestFramework;
    using Graph;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GraphServiceTests
    {
        /// <summary>
        /// The endpoint address for the Microsoft Graph service.
        /// </summary>
        private const string GraphServiceEndpoint = "https://graph.microsoft.com";

        [TestMethod]
        public async Task GetSecureScoreAsync()
        {
            IGraphService graph;
            List<SecureScore> score;

            try
            {
                using (MockContext context = MockContext.Start(GetType().Name))
                {
                    HttpMockServer.Initialize(GetType().Name, "GetSecureScoreAsync", HttpRecorderMode.Playback);

                    graph = new GraphService(
                        new Uri(GraphServiceEndpoint),
                        new TestServiceCredentials("STUB_TOKEN"),
                        HttpMockServer.CreateInstance());

                    score = await graph.GetSecureScoreAsync(
                        1,
                        default(CancellationToken)).ConfigureAwait(false);

                    Assert.IsNotNull(score);
                    Assert.AreEqual(score.Count, 1);

                    context.Stop();
                }
            }
            finally
            {
                graph = null;
                score = null;
            }
        }


        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public async Task GetSecureScoreInvalidPeriodAsync()
        {
            IGraphService graph;

            try
            {
                using (MockContext context = MockContext.Start(GetType().Name))
                {
                    HttpMockServer.Initialize(GetType().Name, "GetSecureScoreInvalidPeriodAsync", HttpRecorderMode.Playback);

                    graph = new GraphService(
                        new Uri(GraphServiceEndpoint),
                        new TestServiceCredentials("STUB_TOKEN"),
                        HttpMockServer.CreateInstance());

                    await graph.GetSecureScoreAsync(
                        0,
                        default(CancellationToken)).ConfigureAwait(false);

                    context.Stop();
                }
            }
            finally
            {
                graph = null;
            }
        }
    }
}