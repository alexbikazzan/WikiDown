using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

using Raven.Client;
using WikiDown.Security;
using WikiDown.Website.ApiModels;
using WikiDown.Website.Security;

namespace WikiDown.Website.Controllers.Api
{
    [Authorize(Roles = ArticleAccessLevel.Admin)]
    [RoutePrefix("api/accounts")]
    public class AccountsController : ApiControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<IReadOnlyList<AccountUserApiModel>> GetUserList()
        {
            var users = await this.UserManager.Users.ToListAsync();

            var accountUsers = users.Select(x => new AccountUserApiModel(x));

            return accountUsers.ToList();
        }

        //[HttpGet]
        //[Route("{username}")]
        //public async Task<AccountUserApiModel> GetUser([FromUri] string username)
        //{
        //    var user = await this.GetEnsuredWikiDownUser(username);

        //    return new AccountUserApiModel(user);
        //}

        [HttpPost]
        [Route("")]
        public async Task<AccountUserApiModel> SaveUser([FromBody] AccountUserApiModel formData)
        {
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
            if (user.Roles.Contains(ArticleAccessLevel.Root) && !this.User.IsInRole(ArticleAccessLevel.Root))
            {
                throw new ArgumentOutOfRangeException("username", "Non-root-user cannot delete root-user.");
            }


            //await this.UserManager.DeleteAsync(user);
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