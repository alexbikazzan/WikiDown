using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SquishIt.Framework;
using WikiDown.Markdown;
using WikiDown.Website.Areas.Assets.Models;
using WikiDown.Website.Controllers;

namespace WikiDown.Website.Areas.Assets.Controllers
{
    [RouteArea(AreaNames.Assets, AreaPrefix = "assets")]
    public class AssetsController : WikiDownControllerBase
    {
        private const string CssContentType = "text/css";

        private const string JavaScriptContentType = "text/javascript";

        [Route("bundles/{name}_{hash}.css")]
        public ActionResult BundledCss(string name)
        {
            try
            {
                string bundleContent = Bundle.Css().RenderCached(name);

                return this.Content(bundleContent, CssContentType);
            }
            catch (KeyNotFoundException ex)
            {
                string message = String.Format("Could not find CSS-bundle named '{0}'.", name);
                throw new KeyNotFoundException(message, ex);
            }
        }

        [Route("bundles/{name}_{hash}.js")]
        public ActionResult BundledJs(string name)
        {
            try
            {
                string bundleContent = Bundle.JavaScript().RenderCached(name);

                return this.Content(bundleContent, JavaScriptContentType);
            }
            catch (KeyNotFoundException ex)
            {
                string message = String.Format("Could not find CSS-bundle named '{0}'.", name);
                throw new KeyNotFoundException(message, ex);
            }
        }

        // TODO: Add hash to URL
        [Route("wikidown/index.js")]
        public ActionResult WikiDownJs()
        {
            var model = new AssetsWikiDownJsViewModel(ConverterHooksConfig.Default);

            this.Response.ContentType = JavaScriptContentType;
            return this.View(model);
        }
    }
}