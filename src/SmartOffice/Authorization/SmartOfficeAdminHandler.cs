// -----------------------------------------------------------------------
// <copyright file="SmartOfficeAdminHandler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Authorization
{
    using System.Threading.Tasks;
    using AspNetCore.Authorization;

    public class SmartOfficeAdminHandler : AuthorizationHandler<SmartOfficeAdminRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            SmartOfficeAdminRequirement requirement)
        {
            if (context.User.IsInRole("Admins"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}