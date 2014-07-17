using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiDown.Markdown
{
    public class ConverterHooksConfig
    {
        private static readonly Lazy<IReadOnlyCollection<IConverterHook>> DefaultConverterHooksLazy;

        private readonly List<IConverterHook> converterHooks;

        static ConverterHooksConfig()
        {
            DefaultConverterHooksLazy =
                new Lazy<IReadOnlyCollection<IConverterHook>>(() => GetDefaultConverterHooks().ToList());
        }

        public ConverterHooksConfig(IEnumerable<RegexReplaceConverterHook> converterHooks = null)
        {
            converterHooks = converterHooks ?? Enumerable.Empty<RegexReplaceConverterHook>();

            this.converterHooks = DefaultConverterHooks.Concat(converterHooks).ToList();
        }

        public static IReadOnlyCollection<IConverterHook> DefaultConverterHooks
        {
            get
            {
                return DefaultConverterHooksLazy.Value;
            }
        }

        public IReadOnlyCollection<IConverterHook> PreConversionHooks
        {
            get
            {
                return this.converterHooks.Where(x => x.Type == ConverterHookType.PreConversion).ToList();
            }
        }

        public IReadOnlyCollection<IConverterHook> PostConversionHooks
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

        public void Register(RegexReplaceConverterHook converterHook)
        {
            this.converterHooks.Add(converterHook);
        }

        public void RegisterPreConversion(
            string regexPattern,
            string regexReplace,
            bool ignoreCase = true,
            bool multiline = true)
        {
            var converterHook = new RegexReplaceConverterHook(
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
            var converterHook = new RegexReplaceConverterHook(
                regexPattern,
                regexReplace,
                ConverterHookType.PostConversion,
                ignoreCase,
                multiline);

            this.Register(converterHook);
        }

        private static IEnumerable<IConverterHook> GetDefaultConverterHooks()
        {
            // https://help.github.com/articles/github-flavored-markdown

            var strikeThrough = new RegexReplaceConverterHook(
                @"~~(.*)~~",
                "<del>$1</del>",
                ConverterHookType.PreConversion);
            yield return strikeThrough;

            var shortWikiLinks = new RegexReplaceConverterHook(
                @"(?:\[([^\]]+)\]\(([^\)]+)\))",
                @"<a href=""/wiki/$2/"">$1</a>",
                ConverterHookType.PreConversion);
            yield return shortWikiLinks;

            var wikiLinks = new RegexReplaceConverterHook(
                @"^<a([^>]+)href=""(\/[^""]+)""([^>]*)>([^<]*)<\/a>$",
                @"<a$1href=""/wiki$2""$3>$4</a>",
                ConverterHookType.PreConversion);
            yield return wikiLinks;

            for (int i = 5; i >= 1; i--)
            {
                string pattern = string.Format(@"^<h{0}([^>]*)>([^<]*)<\/h{0}>$", i);
                string replace = string.Format("<h{0}$1>$2</h{0}>", i + 1);
                yield return new RegexReplaceConverterHook(pattern, replace, ConverterHookType.PostConversion);
            }

            var externalLinks =
                new RegexReplaceConverterHook(
                    @"<a((?:[^>]*)href=""(?:https?|ftp):\/\/(?:[^""]*)""[^>]*)>([^<]*)<\/a>",
                    @"<a$1 class=""external-link"">$2<small class=""glyphicon glyphicon-globe""></small></a>",
                    ConverterHookType.PostConversion);
            yield return externalLinks;

            yield return new HeadingsConverterHook();

            yield return new TocConverterHook();
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