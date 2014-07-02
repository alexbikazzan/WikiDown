using System;

using SquishIt.Framework;
using WikiDown.Website.Controllers;

namespace WikiDown.Website
{
    public static class BundleConfig
    {
        public static void RegisterBundles()
        {
            RegisterCssBundles();
            RegisterJsBundles();
        }

        private static void RegisterCssBundles()
        {
            var siteCss = Bundle.Css();
            var editCss = Bundle.Css();

            ConfigMultiple(x => x.Add("~/Content/Site.css"), siteCss, editCss);

            siteCss.AsCached("site", "~/bundles/site_#.css");
            editCss.Add("~/Content/Libraries/pagedown/Markdown.css")
                .AsCached("edit", "~/" + AssetsController.BundlesBasePath + "edit_#.css");
        }

        private static void RegisterJsBundles()
        {
            var siteJs = Bundle.JavaScript();
            var editJs = Bundle.JavaScript();

            editJs.AddDirectory("~/Content/Libraries/pagedown")
                .Add("~/" + AssetsController.WikiDownAssetsBasePath + "converter-hooks.js")
                .AddDirectory("~/Content/App/", recursive: false)
                .AddDirectory("~/Content/App/");

            ConfigMultiple(x => x.Add("~/Content/Site.js"), siteJs, editJs);

            siteJs.AsCached("site", "~/" + AssetsController.BundlesBasePath + "site_#.js");
            editJs.AsCached("edit", "~/" + AssetsController.BundlesBasePath + "edit_#.js");
        }

        private static void ConfigMultiple<TBundle>(Action<TBundle> configAction, params TBundle[] bundles)
        {
            foreach (var bundle in bundles)
            {
                configAction(bundle);
            }
        }
    }
}