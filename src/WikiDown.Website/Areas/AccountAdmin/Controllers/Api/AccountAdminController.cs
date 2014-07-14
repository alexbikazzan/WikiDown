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
            var currentUserAccessLevel = this.User.GetAccessLevel();
            
            var users = await this.UserManager.Users.ToListAsync();

            var accountUsers = from user in users
                               let userAccessLevel = ArticleAccessHelper.GetAccessLevel(user.Roles)
                               where
                                   userAccessLevel <= currentUserAccessLevel || user.UserName == this.User.Identity.Name
                               orderby user.UserName
                               select new AccountUserApiModel(user);

            return accountUsers.ToList();
        }

        [HttpPost]
        [Route("")]
        public async Task<AccountUserApiModel> SaveUser([FromBody] AccountUserApiModel formData)
        {
            if (formData == null)
            {
                throw new ArgumentNullException("formData");
            }

            if (formData.UserName == ArticleAccessHelper.RootAccountName
                && this.User.Identity.Name != ArticleAccessHelper.RootAccountName)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            var savedUser = await formData.Save(this.User, this.UserManager);
            return new AccountUserApiModel(savedUser);
        }

        [HttpDelete]
        [Route("{username}")]
        public async Task DeleteUser([FromUri] string username)
        {
            var user = await this.GetEnsuredWikiDownUser(username);

            if (user.UserName == this.User.Identity.Name)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            if (user.UserName == ArticleAccessHelper.RootAccountName)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            var currentUserAccessLevel = this.User.GetAccessLevel();
            var userAccessLevel = ArticleAccessHelper.GetAccessLevel(user.Roles);

            if (userAccessLevel > currentUserAccessLevel)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
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