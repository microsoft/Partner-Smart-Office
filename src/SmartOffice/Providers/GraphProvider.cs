// -----------------------------------------------------------------------
// <copyright file="GraphProvider.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Graph;
    using Models;

    public class GraphProvider : IGraphProvider
    {
        /// <summary>
        /// Provides access to the Microsoft Graph.
        /// </summary>
        private readonly IGraphServiceClient client;

        /// <summary>
        /// Identifier of the customer.
        /// </summary>
        private readonly string customerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphProvider"/> class.
        /// </summary>
        /// <param name="customerId">Identifier for customer whose resources are being accessed.</param>
        public GraphProvider(string authority, string clientId, string clientSecret, string customerId)
        {
            this.customerId = customerId;

            client = new GraphServiceClient(new AuthenticationProvider(authority, clientId, clientSecret, customerId));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphClient"/> class.
        /// </summary>
        /// <param name="client">Provides the ability to interact with the Microsoft Graph.</param>
        public GraphProvider(IGraphServiceClient client)
        {
            this.client = client;
        }

        public async Task<IList<Role>> GetRolesAsync(string objectId)
        {
            IUserMemberOfCollectionWithReferencesPage directoryGroups;
            List<Role> roles;
            List<DirectoryRole> directoryRoles;
            bool morePages;

            try
            {
                directoryGroups = await client.Users[objectId].MemberOf.Request().GetAsync().ConfigureAwait(false);
                roles = new List<Role>();

                do
                {
                    directoryRoles = directoryGroups.CurrentPage.OfType<DirectoryRole>().ToList();

                    if (directoryRoles.Count > 0)
                    {
                        roles.AddRange(directoryRoles.Select(r => new Role
                        {
                            Description = r.Description,
                            DisplayName = r.DisplayName
                        }));
                    }

                    morePages = directoryGroups.NextPageRequest != null;

                    if (morePages)
                    {
                        directoryGroups = await directoryGroups.NextPageRequest.GetAsync().ConfigureAwait(false);
                    }
                }
                while (morePages);

                return roles;
            }
            //catch (Exception)
            //{
            //    return null;
            //}
            finally
            {
                directoryGroups = null;
                directoryRoles = null;
            }
        }
    }
}