using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

using SquishIt.Framework;

namespace WikiDown.Website
{
    public static class HtmlHelperBundlesExtensions
    {
        public static IHtmlString BundledCss(this HtmlHelper htmlHelper, string name)
        {
            try
            {
                string assetTag = Bundle.Css().RenderCachedAssetTag(name);
                return new HtmlString(assetTag);
            }
            catch (KeyNotFoundException ex)
            {
                string message = string.Format("Could not find CSS-bundle named '{0}'.", name);
                throw new KeyNotFoundException(message, ex);
            }
        }

        public static IHtmlString BundledJs(this HtmlHelper htmlHelper, string name)
        {
            try
            {
                string assetTag = Bundle.JavaScript().RenderCachedAssetTag(name);
                return new HtmlString(assetTag);
            }
            catch (KeyNotFoundException ex)
            {
                string message = string.Format("Could not find CSS-bundle named '{0}'.", name);
                throw new KeyNotFoundException(message, ex);
            }
        }
    }
}