using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Identity.RavenDB.Stores;
using Microsoft.AspNet.Identity;
using Raven.Client;

namespace WikiDown.Website.Security
{
    public class WikiDownRavenUserStore : RavenUserStore<WikiDownUser>, IUserRoleStore<WikiDownUser>
    {
        private static readonly Task EmptyTaskResult = Task.FromResult<object>(null);

        public WikiDownRavenUserStore(IAsyncDocumentSession documentSession)
            : base(documentSession)
        {
        }

        Task IUserRoleStore<WikiDownUser, string>.AddToRoleAsync(WikiDownUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            user.AddRole(roleName);

            return EmptyTaskResult;
        }

        Task IUserRoleStore<WikiDownUser, string>.RemoveFromRoleAsync(WikiDownUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            user.RemoveRole(roleName);

            return EmptyTaskResult;
        }

        Task<IList<string>> IUserRoleStore<WikiDownUser, string>.GetRolesAsync(WikiDownUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult(user.Roles.ToList() as IList<string>);
        }

        Task<bool> IUserRoleStore<WikiDownUser, string>.IsInRoleAsync(WikiDownUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            return Task.FromResult(user.Roles.Contains(roleName));
        }
    }
}