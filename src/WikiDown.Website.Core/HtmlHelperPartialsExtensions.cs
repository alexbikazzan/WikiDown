using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

using WikiDown.Website.ViewModels;

namespace WikiDown.Website
{
    public static class HtmlHelperPartialsExtensions
    {
        public static IHtmlString ArticleNavigation(this HtmlHelper html, WikiArticleViewModelBase model)
        {
            return html.Partial("_ArticleNavigation", model);
        }
    }
}