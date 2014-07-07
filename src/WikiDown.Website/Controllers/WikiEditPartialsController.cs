using System.Web.Mvc;

using WikiDown.Security;

namespace WikiDown.Website.Controllers
{
    public class WikiEditPartialsController : ControllerBase
    {
        [Authorize(Roles = ArticleAccessLevel.Admin)]
        public ActionResult Admin()
        {
            return this.View();
        }

        [Authorize(Roles = ArticleAccessLevel.Editor)]
        public ActionResult Edit()
        {
            return this.View();
        }

        [Authorize(Roles = ArticleAccessLevel.Editor)]
        public ActionResult Meta()
        {
            return this.View();
        }
    }
}