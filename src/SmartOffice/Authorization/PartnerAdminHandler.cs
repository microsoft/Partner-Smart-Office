// -----------------------------------------------------------------------
// <copyright file="PartnerAdminHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Authorization
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AspNetCore.Authorization;

    public class PartnerAdminHandler : AuthorizationHandler<PartnerAdminRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PartnerAdminRequirement requirement)
        {

            if (context.User.IsInRole("Company Administrator"))
            {
                if (context.User.HasClaim("IsPartner", "true"))
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}