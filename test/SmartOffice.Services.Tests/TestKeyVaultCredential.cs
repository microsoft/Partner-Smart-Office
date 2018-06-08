// -----------------------------------------------------------------------
// <copyright file="TestKeyVaultCredential.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Tests
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.KeyVault;

    public class TestKeyVaultCredential : KeyVaultCredential
    {
        public TestKeyVaultCredential(KeyVaultClient.AuthenticationCallback authenticationCallback) : base(authenticationCallback)
        {
        }

        public override async Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.FromResult(0).ConfigureAwait(false);
        }
    }
}
