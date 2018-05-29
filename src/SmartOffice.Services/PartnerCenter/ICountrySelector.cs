// -----------------------------------------------------------------------
// <copyright file="ICountrySelector.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.PartnerCenter
{
    public interface ICountrySelector<TOperations>
    {
        TOperations ByCountry(string country);
    }
}