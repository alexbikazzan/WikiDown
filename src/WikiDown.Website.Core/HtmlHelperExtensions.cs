using System.Web;
using System.Web.Mvc;

namespace WikiDown.Website
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString Javascript(this HtmlHelper helper, string src)
        {
            var tag = new TagBuilder("script");
            tag.Attributes["type"] = "text/javascript";
            tag.Attributes["src"] = HttpUtility.HtmlAttributeEncode(src);

            return new HtmlString(tag.ToString());
        }

        public static IHtmlString JavascriptConditional(this HtmlHelper helper, string srcWithoutExtension)
        {
            string srcFormat = DevEnvironment.IsDebug ? "{0}.js" : "{0}.min.js";
            string src = string.Format(srcFormat, srcWithoutExtension);

            return helper.Javascript(src);
        }

        public static IHtmlString Stylesheet(this HtmlHelper helper, string href)
        {
            var tag = new TagBuilder("link");
            tag.Attributes["rel"] = "stylesheet";
            tag.Attributes["type"] = "text/css";
            tag.Attributes["href"] = HttpUtility.HtmlAttributeEncode(href);

            return new HtmlString(tag.ToString(TagRenderMode.SelfClosing));
        }

        public static IHtmlString StylesheetConditional(this HtmlHelper helper, string hrefWithoutExtension)
        {
            string hrefFormat = DevEnvironment.IsDebug ? "{0}.css" : "{0}.min.css";
            string href = string.Format(hrefFormat, hrefWithoutExtension);

            return helper.Stylesheet(href);
        }
    }
}