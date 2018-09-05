// -----------------------------------------------------------------------
// <copyright file="KeyVaultTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Azure.KeyVault;
    using KeyVault;
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class KeyVaultTests : IDisposable
    {
        /// <summary>
        /// The Key Vault endpoint address to be used for testing.
        /// </summary>
        private const string KeyVaultEndpoint = "https://smartoffice.test";

        /// <summary>
        /// Provides the ability to perform cryptographic key operations and vault operations 
        /// against the Key Vault service.
        /// </summary>
        private readonly IKeyVaultClient keyVaultClient;

        /// <summary>
        /// Flag indicating whether or not this object has been disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultTests" /> class.
        /// </summary>
        public KeyVaultTests()
        {
            keyVaultClient = new KeyVaultClient(
                new TestKeyVaultCredential(GetAccessToken),
                new DelegatingHandler[] { new TestHttpMessageHandler() });
        }

        /// <summary>
        /// Validates that the GetSecretAsync function returns a string value.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Task" /> class that represents the asynchronous operation.
        /// </returns>
        [TestMethod]
        public async Task GetSecretTestAsync()
        {
            KeyVaultService service;
            string actual;

            try
            {
                service = new KeyVaultService(keyVaultClient);

                actual = await service.GetSecretAsync(KeyVaultEndpoint, "UnitTest").ConfigureAwait(false);

                Assert.AreEqual("AmazingSecret", actual);
            }
            finally
            {
                service = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                keyVaultClient?.Dispose();
            }

            disposed = true;
        }

        /// <summary>
        /// Gets an access token to be used when requesting data from Azure Key Vault.
        /// </summary>
        /// <param name="authority"></param>
        /// <param name="resource"></param>
        /// <param name="scope"></param>
        /// <returns>A string representing the requested access token.</returns>
        private static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            await Task.FromResult(0).ConfigureAwait(false);

            return string.Empty;
        }
    }
}