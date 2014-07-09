using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

using WikiDown.Security;
using WikiDown.Website.ApiModels;
using WikiDown.Website.Security;

namespace WikiDown.Website.Controllers.Api
{
    [RoutePrefix("api/meta")]
    public class MetaController : WikiDownApiControllerBase
    {
        [Authorize]
        [HttpGet]
        [Route("roles")]
        public async Task<IReadOnlyCollection<IdTextApiModel>> ListRoles()
        {
            var user = await this.GetEnsuredWikiDownUser(this.User.Identity.Name);

            return user.Roles.Select(GetIdText).ToList();
        }

        [HttpGet]
        [Route("roles-all")]
        public IReadOnlyCollection<IdTextApiModel> ListAllRoles()
        {
            var allRoles = new[] { ArticleAccessHelper.Anonymous }.Concat(ArticleAccessHelper.AdminRoles);

            return allRoles.Select(GetIdText).ToList();
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

        private static IdTextApiModel GetIdText(string roleName)
        {
            ArticleAccessLevel accessLevel;
            if (!Enum.TryParse(roleName, out accessLevel))
            {
                accessLevel = ArticleAccessLevel.Anonymous;
            }

            var accessLevelValue = (int)accessLevel;
            return new IdTextApiModel(accessLevelValue, accessLevel.ToString());
        }
    }
}