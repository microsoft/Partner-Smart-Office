// -----------------------------------------------------------------------
// <copyright file="CustomerBillingProfile.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter.Customers
{
    public sealed class CustomerBillingProfile
    {
        public string CompanyName { get; set; }

        public string Culture { get; set; }

        public Address DefaultAddress { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string Id { get; set; }

        public string LastName { get; set; }

        public string Language { get; set; }
    }
}