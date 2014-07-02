using System;
using System.Web.Mvc;

namespace WikiDown.Website.Controllers
{
    [Route("site/{action=index}/{id=guid?}", Name = RouteNames.Site)]
    public class SiteController : ControllerBase
    {
        [Route("~/", Name = RouteNames.Empty)]
        public ActionResult Index()
        {
            return this.RedirectToRoute(RouteNames.WikiList);
        }

        public ActionResult About()
        {
            throw new NotImplementedException();
        }
    }
}