// -----------------------------------------------------------------------
// <copyright file="SmartOfficeExtensionConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Microsoft.Partner.SmartOffice.Functions.ClassMaps
{
    using CsvHelper.Configuration;
    using Models.Graph;

    public class ControlListEntryClassMap : ClassMap<ControlListEntry>
    {
        public ControlListEntryClassMap()
        {
            Map(m => m.ActionCategory).Name("Action Category");
            Map(m => m.ActionUrl).Name("Action Url");
            Map(m => m.Baseline).Name("Baseline");
            Map(m => m.Deprecated).Name("Deprecated");
            Map(m => m.Description).Name("Description");
            Map(m => m.Enablement).Name("Enablement");
            Map(m => m.ImplementationCost).Name("Implementation Cost");
            Map(m => m.Name).Name("Name");
            Map(m => m.ReferenceId).Name("Reference Id");
            Map(m => m.RemediationChange).Name("Remediation Change");
            Map(m => m.RemediationImpact).Name("Remediation Impact");
            Map(m => m.StackRank).Name("Stack Rank");
            Map(m => m.Threats).Name("Threats");
            Map(m => m.Tier).Name("Tier");
            Map(m => m.UserImpact).Name("User Impact");
            Map(m => m.Workload).Name("Workload");
        }
    }
}