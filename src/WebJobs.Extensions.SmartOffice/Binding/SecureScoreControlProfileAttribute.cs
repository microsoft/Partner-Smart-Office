// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Azure.WebJobs.Extensions.SmartOffice
{
    using System;
    using Description;

    /// <summary>
    /// Represents the an attribute used to bind a list of Secure Score control profiles to a parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    [Binding]
    public sealed class SecureScoreControlProfileAttribute : TokenBaseAttribute
    {
    }
}