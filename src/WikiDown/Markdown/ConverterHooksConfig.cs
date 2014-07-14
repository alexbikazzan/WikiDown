using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiDown.Markdown
{
    public class ConverterHooksConfig
    {
        private static readonly Lazy<List<ConverterHook>> DefaultConverterHooksLazy;

        static ConverterHooksConfig()
        {
            DefaultConverterHooksLazy = new Lazy<List<ConverterHook>>(() => GetDefaultConverterHooks().ToList());
        }

        private readonly List<ConverterHook> converterHooks;

        public ConverterHooksConfig(IEnumerable<ConverterHook> converterHooks = null)
        {
            converterHooks = converterHooks ?? Enumerable.Empty<ConverterHook>();

            this.converterHooks = DefaultConverterHooks.Concat(converterHooks).ToList();
        }

        public static IReadOnlyCollection<ConverterHook> DefaultConverterHooks
        {
            get
            {
                return DefaultConverterHooksLazy.Value;
            }
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

        public IReadOnlyCollection<ConverterHook> ServerSideHooks
        {
            get
            {
                return this.converterHooks.Where(x => x.Type == ConverterHookType.ServerSide).ToList();
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

        public string ApplyServerSide(string html)
        {
            return this.ServerSideHooks.Aggregate(html, (current, hook) => hook.Apply(current));
        }

        private static IEnumerable<ConverterHook> GetDefaultConverterHooks()
        {
            // https://help.github.com/articles/github-flavored-markdown

            var strikeThrough = new ConverterHook(@"~~(.*)~~", "<del>$1</del>", ConverterHookType.PreConversion);
            yield return strikeThrough;

            var wikiLinks = new ConverterHook(
                @"^<a([^>]+)href=""(\/[^""]+)""([^>]*)>([^<]*)<\/a>$",
                @"<a$1href=""/wiki$2""$3>$4</a>",
                ConverterHookType.PreConversion);
            yield return wikiLinks;

            for (int i = 5; i >= 1; i--)
            {
                string pattern = string.Format(@"^<h{0}([^>]*)>([^<]*)<\/h{0}>$", i);
                string replace = string.Format("<h{0}$1>$2</h{0}>", i + 1);
                yield return new ConverterHook(pattern, replace, ConverterHookType.PostConversion);
            }

            var externalLinks = new ConverterHook(
                @"<a((?:[^>]*)href=""(?:https?|ftp):\/\/(?:[^""]*)""[^>]*)>([^<]*)<\/a>",
                @"<a$1 class=""external-link"">$2</a> <small class=""glyphicon glyphicon-globe""></small>",
                ConverterHookType.PostConversion);
            yield return externalLinks;

            yield return new TocServerSideHook();
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