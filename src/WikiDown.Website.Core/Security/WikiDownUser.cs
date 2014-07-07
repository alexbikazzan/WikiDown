using System;
using System.Collections.Generic;
using System.Linq;

using AspNet.Identity.RavenDB.Entities;
using Microsoft.AspNet.Identity;
using Raven.Imports.Newtonsoft.Json;

namespace WikiDown.Website.Security
{
    public class WikiDownUser : RavenUser, IUser
    {
        private static readonly string WikiDownUserTypeName = typeof(WikiDownUser).Name;

        private HashSet<string> roles;

        [JsonConstructor]
        public WikiDownUser(string userName)
            : base(userName)
        {
            this.roles = new HashSet<string>();
        }

        public WikiDownUser(string userName, string email)
            : base(userName, email)
        {
            this.roles = new HashSet<string>();
        }

        public IEnumerable<string> Roles
        {
            get
            {
                return this.roles;
            }
            set
            {
                if (this.roles == null)
                {
                    this.roles = new HashSet<string>();
                }

                foreach (var val in value ?? Enumerable.Empty<string>())
                {
                    this.roles.Add(val);
                }
            }
        }

        public void AddRole(string roleName)
        {
            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            if (!this.roles.Contains(roleName))
            {
                this.roles.Add(roleName);
            }
        }

        public void RemoveRole(string roleName)
        {
            if (roleName == null)
            {
                throw new ArgumentNullException("roleName");
            }

            if (this.roles.Contains(roleName))
            {
                this.roles.Remove(roleName);
            }
        }

        //internal static string GenerateKey(string userName)
        //{
        //    return string.Format("{0}/{1}", WikiDownUserTypeName, userName);
        //}
    }
}