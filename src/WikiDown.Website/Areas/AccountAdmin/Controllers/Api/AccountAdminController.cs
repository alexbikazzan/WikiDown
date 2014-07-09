using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

using Raven.Client;
using WikiDown.Security;
using WikiDown.Website.Areas.AccountAdmin.Models;
using WikiDown.Website.Controllers.Api;
using WikiDown.Website.Security;

namespace WikiDown.Website.Areas.AccountAdmin.Controllers.Api
{
    [Authorize(Roles = ArticleAccessHelper.Admin)]
    [RoutePrefix("api/admin")]
    public class AccountsController : WikiDownApiControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<IReadOnlyList<AccountUserApiModel>> GetUserList()
        {
            var users = await this.UserManager.Users.ToListAsync();

            var accountUsers = users.Select(x => new AccountUserApiModel(x));

            return accountUsers.ToList();
        }

        [HttpPost]
        [Route("")]
        public async Task<AccountUserApiModel> SaveUser([FromBody] AccountUserApiModel formData)
        {
            // TODO: Ensure lower-auth users cannot upgrade to admin/root
            // TODO: Ensure lower-auth users cannot change higher-auth user's settings
            if (formData == null)
            {
                throw new ArgumentNullException("formData");
            }

            await formData.Save(this.UserManager);
            return formData;
        }

        [HttpDelete]
        [Route("{username}")]
        public async Task DeleteUser([FromUri] string username)
        {
            var user = await this.GetEnsuredWikiDownUser(username);

            if (user.UserName == this.User.Identity.Name)
            {
                throw new ArgumentOutOfRangeException("username", "Cannot delete own account.");
            }
            if (user.Roles.Contains(ArticleAccessHelper.Root) && !this.User.IsInRole(ArticleAccessHelper.Root))
            {
                throw new ArgumentOutOfRangeException("username", "Non-root-user cannot delete root-user.");
            }

            await this.UserManager.DeleteAsync(user);
        }

        private async Task<WikiDownUser> GetEnsuredWikiDownUser(string username)
        {
            var user = await this.UserManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return user;
        }
    }
}