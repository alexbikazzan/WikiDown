using System.Web.Mvc;

using WikiDown.Security;
using WikiDown.Website.Controllers;

namespace WikiDown.Website.Areas.AccountAdmin.Controllers
{
    [Authorize(Roles = ArticleAccessHelper.Admin)]
    [RouteArea(AreaNames.AccountAdmin, AreaPrefix = "admin")]
    public class AccountAdminController : WikiDownControllerBase
    {
        [Route("", Name = RouteNames.AccountAdmin)]
        public ActionResult Index()
        {
            return this.View();
        }
    }
}