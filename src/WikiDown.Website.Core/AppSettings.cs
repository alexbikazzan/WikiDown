using System;
using System.Configuration;

namespace WikiDown.Website
{
    public static class AppSettings
    {
        private const string AppSettingPrefix = "wikidown";

        private const string AppSettingFormat = "{0}:{1}";

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