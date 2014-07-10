using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace WikiDown.Website
{
    public static class HtmlHelperExtensions
    {
        public static bool HasValidationMessage(this HtmlHelper htmlHelper, string modelName)
        {
            var message = htmlHelper.ValidationMessage(modelName);
            return (message != null) && !string.IsNullOrWhiteSpace(message.ToHtmlString());
        }

        public static IHtmlString Javascript(this HtmlHelper helper, string src)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);

            var tag = new TagBuilder("script");
            tag.Attributes["type"] = "text/javascript";
            tag.Attributes["src"] = urlHelper.Content(src);

            return new HtmlString(tag.ToString());
        }

        public static IHtmlString JavascriptConditional(this HtmlHelper htmlHelper, string srcWithoutExtension)
        {
            string srcFormat = DevEnvironment.IsDebug ? "{0}.js" : "{0}.min.js";
            string src = string.Format(srcFormat, srcWithoutExtension);

            return htmlHelper.Javascript(src);
        }

        public static IHtmlString Stylesheet(this HtmlHelper htmlHelper, string href)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);

            var tag = new TagBuilder("link");
            tag.Attributes["rel"] = "stylesheet";
            tag.Attributes["type"] = "text/css";
            tag.Attributes["href"] = urlHelper.Content(href);

            return new HtmlString(tag.ToString(TagRenderMode.SelfClosing));
        }

        public static IHtmlString StylesheetConditional(this HtmlHelper htmlHelper, string hrefWithoutExtension)
        {
            string hrefFormat = DevEnvironment.IsDebug ? "{0}.css" : "{0}.min.css";
            string href = string.Format(hrefFormat, hrefWithoutExtension);

            return htmlHelper.Stylesheet(href);
        }
    }
}