using System.Collections.Generic;
using System.Linq;
using System.Web;

using WikiDown.Markdown;

namespace WikiDown.Website.Areas.Assets.Models
{
    public class AssetsWikiDownJsViewModel
    {
        public AssetsWikiDownJsViewModel(
            IEnumerable<ConverterHook> preConversionHooks,
            IEnumerable<ConverterHook> postConversionHooks,
            IEnumerable<ConverterHook> serverSideHooks)
        {
            this.PreConversionHooks = preConversionHooks.Select(ConverterHookItem.ForPreConversion).ToList();
            this.PostConversionHooks = postConversionHooks.Select(ConverterHookItem.ForPostConversion).ToList();
            this.ServerSideHooks =
                serverSideHooks.Where(
                    x => !string.IsNullOrWhiteSpace(x.RegexPattern) && !string.IsNullOrWhiteSpace(x.RegexReplace))
                    .Select(ConverterHookItem.ForPostConversion)
                    .ToList();
        }

        public IReadOnlyCollection<ConverterHookItem> PreConversionHooks { get; private set; }

        public IReadOnlyCollection<ConverterHookItem> PostConversionHooks { get; private set; }

        public IReadOnlyCollection<ConverterHookItem> ServerSideHooks { get; private set; }

        public class ConverterHookItem
        {
            private ConverterHookItem(ConverterHook hook, string arrayName)
            {
                this.ArrayName = arrayName;

                this.RegexPattern = new HtmlString(hook.RegexPattern); //Regex.Escape(hook.RegexPattern);
                this.RegexFlags = hook.RegexFlags;
                this.RegexReplace = new HtmlString(hook.RegexReplace);
            }

            public string ArrayName { get; private set; }

            public IHtmlString RegexPattern { get; private set; }

            public string RegexFlags { get; private set; }

            public IHtmlString RegexReplace { get; private set; }

            public static ConverterHookItem ForPreConversion(ConverterHook hook)
            {
                return new ConverterHookItem(hook, "preConversions");
            }

            public static ConverterHookItem ForPostConversion(ConverterHook hook)
            {
                return new ConverterHookItem(hook, "postConversions");
            }
        }
    }
}