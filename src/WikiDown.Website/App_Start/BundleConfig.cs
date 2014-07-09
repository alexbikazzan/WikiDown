using System;

using SquishIt.Framework;

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

            siteCss.AsCached("site", "~/assets/bundles/site_#.css");

            wikiArticleEditCss.Add("~/Content/Libraries/pagedown/Markdown.css")
                .AsCached("wiki-edit", "~/assets/bundles/wiki-edit_#.css");
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

            accountAdminJs.AddDirectory("~/Areas/AccountAdmin/Content/App/", recursive: false)
                .AddDirectory("~/Areas/AccountAdmin/Content/App/");

            wikiArticleEditJs.AddDirectory("~/Content/Libraries/pagedown")
                .AddDirectory("~/Areas/WikiEdit/Content/App/", recursive: false)
                .AddDirectory("~/Areas/WikiEdit/Content/App/");

            //infoJs.Add("~/Content/Info.js");

            ConfigMultiple(x => x.Add("~/Content/Site.js"), siteJs, wikiArticleEditJs, infoJs);

            siteJs.AsCached("site", GetCachedFilePath("site"));

            infoJs.AsCached("info", GetCachedFilePath("info"));

            accountAdminJs.AsCached("account-admin", GetCachedFilePath("account-admin"));

            wikiArticleEditJs.AsCached("wiki-edit", GetCachedFilePath("wiki-edit"));
        }

        private static string GetCachedFilePath(string filename)
        {
            return string.Format("~/assets/bundles/{0}_#.js", filename);
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