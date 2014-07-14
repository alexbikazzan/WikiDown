using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using WikiDown.Security;
using WikiDown.Website.Security;

namespace WikiDown.Website.Areas.AccountAdmin.Models
{
    public class AccountUserApiModel
    {
        public AccountUserApiModel()
        {
        }

        public AccountUserApiModel(WikiDownUser user)
        {
            this.Email = user.Email;
            this.UserName = user.UserName;

            this.IsRoot = (user.UserName == ArticleAccessHelper.RootAccountName);
            this.AccessLevel = (int)ArticleAccessHelper.GetAccessLevel(user.Roles);
        }

        public int AccessLevel { get; set; }

        public string Email { get; set; }

        public bool IsRoot { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }

        public async Task<WikiDownUser> Save(IPrincipal principal, UserManager<WikiDownUser> userManager)
        {
            var user = await userManager.FindByNameAsync(this.UserName);

            var roles = this.GetRoles(principal, user);

            if (user != null)
            {
                if (user.UserName == principal.Identity.Name)
                {
                    var userAccessLevel = ArticleAccessHelper.GetAccessLevel(user.Roles);
                    if (userAccessLevel < ArticleAccessLevel.Admin)
                    {
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                    }
                }

                user.SetRoles(roles);
                user.SetEmail(this.Email);

                if (!string.IsNullOrWhiteSpace(this.Password))
                {
                    await userManager.RemovePasswordAsync(user.Id);
                    await userManager.AddPasswordAsync(user.Id, this.Password);
                }

                await userManager.UpdateAsync(user);
            }
            else
            {
                user = new WikiDownUser(this.UserName) { Roles = roles };
                user.SetEmail(this.Email);

                await userManager.CreateAsync(user, this.Password);
            }

            return user;
        }

        private IEnumerable<string> GetRoles(IPrincipal principal, WikiDownUser user)
        {
            var userRoles = ArticleAccessHelper.GetRoles(this.AccessLevel);

            if (user != null)
            {
                var userAccessLevel = ArticleAccessHelper.GetAccessLevel(user.Roles);
                var principalAccessLevel = principal.GetAccessLevel();
                if (userAccessLevel > principalAccessLevel)
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }
            }

            return userRoles;
        }
    }
}