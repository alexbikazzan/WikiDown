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
            this.converterHooks = (converterHooks ?? Enumerable.Empty<ConverterHook>()).ToList();
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

        #region Singleton

        private static readonly Lazy<ConverterHooksConfig> CurrentLazy =
            new Lazy<ConverterHooksConfig>(GetDefaultConverterHooksConfig);

        private static ConverterHooksConfig GetDefaultConverterHooksConfig()
        {
            var config = new ConverterHooksConfig();
            
            // TODO: Add WikiDown-converter-hooks

            return config;
        }

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