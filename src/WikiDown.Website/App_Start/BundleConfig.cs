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
            var wikiArticleEditCss = Bundle.Css();

            ConfigMultiple(x => x.Add("~/Content/Site.css"), siteCss, wikiArticleEditCss);

            siteCss.AsCached("site", "~/bundles/site_#.css");

            wikiArticleEditCss.Add("~/Content/Libraries/pagedown/Markdown.css")
                .AsCached("wiki-edit", "~/" + AssetsController.BundlesBasePath + "wiki-edit_#.css");
        }

        private static void RegisterJsBundles()
        {
            var siteJs = Bundle.JavaScript();
            var infoJs = Bundle.JavaScript();

            var accountAdminJs = Bundle.JavaScript();
            var wikiArticleEditJs = Bundle.JavaScript();

            ConfigMultiple(
                x => x.AddDirectory("~/Content/App/_Shared/", recursive: false).AddDirectory("~/Content/App/_Shared/"),
                accountAdminJs,
                wikiArticleEditJs);

            accountAdminJs.AddDirectory("~/Content/App/AccountAdmin/", recursive: false)
                .AddDirectory("~/Content/App/AccountAdmin/");

            wikiArticleEditJs.AddDirectory("~/Content/Libraries/pagedown")
                .Add("~/" + AssetsController.WikiDownAssetsBasePath + "converter-hooks.js")
                .AddDirectory("~/Content/App/WikiEdit", recursive: false)
                .AddDirectory("~/Content/App/WikiEdit");

            infoJs.Add("~/Content/Info.js");

            ConfigMultiple(x => x.Add("~/Content/Site.js"), siteJs, wikiArticleEditJs, infoJs);

            siteJs.AsCached("site", "~/" + AssetsController.BundlesBasePath + "site_#.js");

            infoJs.AsCached("info", "~/" + AssetsController.BundlesBasePath + "info_#.js");

            accountAdminJs.AsCached("account-admin", "~/" + AssetsController.BundlesBasePath + "account-admin_#.js");

            wikiArticleEditJs.AsCached("wiki-edit", "~/" + AssetsController.BundlesBasePath + "wiki-edit_#.js");
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