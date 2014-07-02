using System;
using System.Collections.Generic;
using System.Web.Mvc;

using SquishIt.Framework;
using WikiDown.Markdown;
using WikiDown.Website.ViewModels;

namespace WikiDown.Website.Controllers
{
    public class AssetsController : ControllerBase
    {
        private const string AssetsBasePath = "assets/";

        private const string CssContentType = "text/css";

        private const string JavaScriptContentType = "text/javascript";

        public const string BundlesBasePath = (AssetsBasePath + "bundles/");

        public const string WikiDownAssetsBasePath = (AssetsBasePath + "wikidown/");

        [Route(BundlesBasePath + "{name}_{hash}.css")]
        [Route(BundlesBasePath + "{name}.css")]
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

        [Route(BundlesBasePath + "{name}_{hash}.js")]
        [Route(BundlesBasePath + "{name}.js")]
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

        [Route(WikiDownAssetsBasePath + "converter-hooks.js")]
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