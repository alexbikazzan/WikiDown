using System;
using System.Configuration;

namespace WikiDown.Website
{
    public static class AppSettings
    {
        private const string AppSettingPrefix = "wikidown";

        private const string AppSettingFormat = "{0}:{1}";

        private static readonly Lazy<string> SiteHeaderLazy =
            new Lazy<string>(() => GetAppSetting("SiteHeader") ?? "WikiDown");

        private static readonly Lazy<string> SiteHeaderTitleLazy =
            new Lazy<string>(() => GetAppSetting("SiteHeaderTitle"));

        public static string SiteHeader
        {
            get
            {
                return SiteHeaderLazy.Value;
            }
        }

        public static string SiteHeaderTitle
        {
            get
            {
                return SiteHeaderTitleLazy.Value;
            }
        }

        public static string RootPassword
        {
            get
            {
                return GetAppSetting("RootPassword");
            }
        }

        private static string GetAppSetting(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            string appSettingKey = string.Format(AppSettingFormat, AppSettingPrefix, key);

            return ConfigurationManager.AppSettings[appSettingKey];
        }
    }
}