using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace WikiDown
{
    public static class ArticleSlugUtility
    {
        private static readonly IReadOnlyCollection<SlugRule> EncodeRules;

        private static readonly IReadOnlyCollection<SlugRule> DecodeRules;

        static ArticleSlugUtility()
        {
            EncodeRules = new[] { CharacterReplaceRule(), UrlEncodeDecodeRule() }.ToList();
            DecodeRules = EncodeRules.Reverse().ToList();
        }

        public static string Encode(string articleTitle)
        {
            if (articleTitle == null)
            {
                throw new ArgumentNullException("articleTitle");
            }

            return EncodeRules.Aggregate(articleTitle, (current, rule) => rule.EncodeRule(current));
        }

        public static string Decode(string articleSlug)
        {
            if (articleSlug == null)
            {
                throw new ArgumentNullException("articleSlug");
            }

            return DecodeRules.Aggregate(articleSlug, (current, rule) => rule.DecodeRule(current));
        }

        private static SlugRule CharacterReplaceRule()
        {
            return new SlugRule { EncodeRule = x => x.Replace(' ', '_'), DecodeRule = x => x.Replace('_', ' ') };
        }

        private static SlugRule UrlEncodeDecodeRule()
        {
            return new SlugRule { EncodeRule = WebUtility.UrlEncode, DecodeRule = WebUtility.UrlDecode };
        }

        private class SlugRule
        {
            internal Func<string, string> EncodeRule { get; set; }

            internal Func<string, string> DecodeRule { get; set; }
        }
    }
}