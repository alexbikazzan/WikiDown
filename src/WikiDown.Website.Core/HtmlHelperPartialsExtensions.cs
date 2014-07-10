using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using WikiDown.Website.ViewModels;

namespace WikiDown.Website
{
    public static class HtmlHelperPartialsExtensions
    {
        public static IHtmlString ArticleNavigation(this HtmlHelper htmlHelper, WikiArticleViewModelBase model)
        {
            return htmlHelper.Partial("_ArticleNavigation", model);
        }
    }
}