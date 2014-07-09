using System;
using System.Collections.Generic;
using System.Linq;

using AspNet.Identity.RavenDB.Entities;
using Raven.Imports.Newtonsoft.Json;

namespace WikiDown.Website.Security
{
    public class WikiDownUser : RavenUser
    {
        private HashSet<string> rolesList;

        [JsonConstructor]
        public WikiDownUser(string userName)
            : base(userName)
        {
            this.rolesList = new HashSet<string>();
        }

        public WikiDownUser(string userName, string email)
            : base(userName, email)
        {
            this.rolesList = new HashSet<string>();
        }

        public IEnumerable<string> Roles
        {
            get
            {
                return this.rolesList;
            }
            set
            {
                if (this.rolesList == null)
                {
                    this.rolesList = new HashSet<string>();
                }

                foreach (var val in value ?? Enumerable.Empty<string>())
                {
                    this.rolesList.Add(val);
                }
            }
        }

        public void AddRole(string roleName)
        {
            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            if (!this.rolesList.Contains(roleName))
            {
                this.rolesList.Add(roleName);
            }
        }

        public void RemoveRole(string roleName)
        {
            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            if (this.rolesList.Contains(roleName))
            {
                this.rolesList.Remove(roleName);
            }
        }

        public void SetRoles(IEnumerable<string> roles)
        {
            if (roles == null)
            {
                throw new ArgumentNullException("roles");
            }

            this.rolesList.Clear();
            roles.ToList().ForEach(x => this.rolesList.Add(x));
        }
    }
}