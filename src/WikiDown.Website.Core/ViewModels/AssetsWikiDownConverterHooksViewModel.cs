using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using WikiDown.Markdown;

namespace WikiDown.Website.ViewModels
{
    public class AssetsWikiDownConverterHooksViewModel
    {
        public AssetsWikiDownConverterHooksViewModel(
            IEnumerable<ConverterHook> preConversionHooks,
            IEnumerable<ConverterHook> postConversionHooks)
        {
            this.PreConversionHooks = preConversionHooks.Select(ConverterHookItem.ForPreConversion).ToList();
            this.PostConversionHooks = postConversionHooks.Select(ConverterHookItem.ForPostConversion).ToList();
        }

        public IReadOnlyCollection<ConverterHookItem> PreConversionHooks { get; private set; }

        public IReadOnlyCollection<ConverterHookItem> PostConversionHooks { get; private set; }

        public class ConverterHookItem
        {
            private ConverterHookItem(ConverterHook hook, string arrayName)
            {
                this.ArrayName = arrayName;

                this.RegexPattern = Regex.Escape(hook.RegexPattern);
                this.RegexFlags = hook.RegexFlags;
                this.RegexReplace = Regex.Escape(hook.RegexReplace);
            }

            public string ArrayName { get; private set; }

            public string RegexPattern { get; private set; }

            public string RegexFlags { get; private set; }

            public string RegexReplace { get; private set; }

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