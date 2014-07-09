using System.Web.Mvc;

using WikiDown.Security;
using WikiDown.Website.Controllers;

namespace WikiDown.Website.Areas.WikiEdit.Controllers
{
    [RouteArea(AreaNames.WikiEdit, AreaPrefix = "wikieditpartials")]
    [Route("{action}/{*path}")]
    public class WikiEditPartialsController : WikiDownControllerBase
    {
        [Authorize(Roles = ArticleAccessHelper.Admin)]
        public ActionResult Admin()
        {
            return this.View();
        }

        [Authorize(Roles = ArticleAccessHelper.Editor)]
        public ActionResult Edit()
        {
            return this.View();
        }

        [Authorize(Roles = ArticleAccessHelper.Editor)]
        public ActionResult History()
        {
            return this.View();
        }

        [Authorize(Roles = ArticleAccessHelper.Editor)]
        public ActionResult Meta()
        {
            return this.View();
        }
    }
}