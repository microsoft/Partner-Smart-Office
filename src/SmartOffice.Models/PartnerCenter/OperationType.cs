// -----------------------------------------------------------------------
// <copyright file="OperationType.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.PartnerCenter
{
    using Converters;
    using Newtonsoft.Json;

    public enum OperationType
    {
        UpdateCustomerQualification,
        UpdateSubscription,
        UpgradeSubscription,
        ConvertTrialSubscription,
        AddCustomer,
        UpdateCustomerBillingProfile,
        UpdateCustomerPartnerContractCompanyName,
        UpdateCustomerSpendingBudget,
        DeleteCustomer,
        RemovePartnerCustomerRelationship,
        CreateOrder,
        UpdateOrder,
        CreateCustomerUser,
        DeleteCustomerUser,
        UpdateCustomerUser,
        UpdateCustomerUserLicenses,
        ResetCustomerUserPassword,
        UpdateCustomerUserPrincipalName,
        RestoreCustomerUser,
        CreateMpnAssociation,
        UpdateMpnAssociation,
    }
}