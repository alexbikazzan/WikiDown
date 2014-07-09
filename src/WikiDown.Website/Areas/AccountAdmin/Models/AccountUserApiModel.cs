using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            this.SetRole(user);
        }

        public int AccessLevel { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }

        public async Task Save(UserManager<WikiDownUser> userManager)
        {
            var roles = this.GetRoles();

            var user = await userManager.FindByNameAsync(this.UserName);
            if (user != null)
            {
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

            this.Password = null;
        }

        public IEnumerable<string> GetRoles()
        {
            if (this.UserName == ArticleAccessManager.RootAccountName)
            {
                return ArticleAccessHelper.RootRoles;
            }

            return ArticleAccessHelper.GetRoles(this.AccessLevel);
        }

        private void SetRole(WikiDownUser user)
        {
            if (user.Roles.Contains(ArticleAccessHelper.Root))
            {
                this.AccessLevel = (int)ArticleAccessLevel.Root;
            }
            else if (user.Roles.Contains(ArticleAccessHelper.Admin))
            {
                this.AccessLevel = (int)ArticleAccessLevel.Admin;
            }
            else if (user.Roles.Contains(ArticleAccessHelper.SuperUser))
            {
                this.AccessLevel = (int)ArticleAccessLevel.SuperUser;
            }
            else if (user.Roles.Contains(ArticleAccessHelper.Editor))
            {
                this.AccessLevel = (int)ArticleAccessLevel.Editor;
            }
        }
    }
}