using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiDown.Markdown
{
    public class ConverterHooksConfig
    {
        private readonly List<ConverterHook> converterHooks;

        public ConverterHooksConfig(IEnumerable<ConverterHook> converterHooks = null)
        {
            converterHooks = converterHooks ?? Enumerable.Empty<ConverterHook>();
            var defaultConverterHooks = GetDefaultConverterHooks();

            this.converterHooks = defaultConverterHooks.Concat(converterHooks).ToList();
        }

        public void Register(ConverterHook converterHook)
        {
            this.converterHooks.Add(converterHook);
        }

        public void RegisterPreConversion(
            string regexPattern,
            string regexReplace,
            bool ignoreCase = true,
            bool multiline = true)
        {
            var converterHook = new ConverterHook(
                regexPattern,
                regexReplace,
                ConverterHookType.PreConversion,
                ignoreCase,
                multiline);

            this.Register(converterHook);
        }

        public void RegisterPostConversion(
            string regexPattern,
            string regexReplace,
            bool ignoreCase = true,
            bool multiline = true)
        {
            var converterHook = new ConverterHook(
                regexPattern,
                regexReplace,
                ConverterHookType.PostConversion,
                ignoreCase,
                multiline);

            this.Register(converterHook);
        }

        public IReadOnlyCollection<ConverterHook> PreConversionHooks
        {
            get
            {
                return this.converterHooks.Where(x => x.Type == ConverterHookType.PreConversion).ToList();
            }
        }

        public IReadOnlyCollection<ConverterHook> PostConversionHooks
        {
            get
            {
                return this.converterHooks.Where(x => x.Type == ConverterHookType.PostConversion).ToList();
            }
        }

        public string ApplyPreConversions(string markdown)
        {
            return this.PreConversionHooks.Aggregate(markdown, (current, hook) => hook.Apply(current));
        }

        public string ApplyPostConversions(string html)
        {
            return this.PostConversionHooks.Aggregate(html, (current, hook) => hook.Apply(current));
        }

        private static IEnumerable<ConverterHook> GetDefaultConverterHooks()
        {
            // https://help.github.com/articles/github-flavored-markdown

            var strikeThrough = new ConverterHook(@"~~(.*)~~", @"<del>$1</del>", ConverterHookType.PreConversion);
            yield return strikeThrough;
        }

        #region Singleton

        private static readonly Lazy<ConverterHooksConfig> CurrentLazy =
            new Lazy<ConverterHooksConfig>(() => new ConverterHooksConfig());

        public static ConverterHooksConfig Default
        {
            get
            {
                return CurrentLazy.Value;
            }
        }

        #endregion
    }
}