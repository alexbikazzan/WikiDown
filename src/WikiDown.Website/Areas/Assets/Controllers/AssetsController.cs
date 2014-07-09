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

        [Route(BundleConfig.BundlesBasePath + "/{name}_{hash}.css")]
        [Route(BundleConfig.BundlesBasePath + "/{name}.css")]
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

        [Route(BundleConfig.BundlesBasePath + "/{name}_{hash}.js")]
        [Route(BundleConfig.BundlesBasePath + "/{name}.js")]
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

        [Route(BundleConfig.WikiDownAssetsBasePath + "/converter-hooks.js")]
        public ActionResult WikiDownConverterHooks()
        {
            var model = new AssetsWikiDownConverterHooksViewModel(
                ConverterHooksConfig.Default.PreConversionHooks,
                ConverterHooksConfig.Default.PostConversionHooks);

            this.Response.ContentType = JavaScriptContentType;
            return this.View(model);
        }
    }
}