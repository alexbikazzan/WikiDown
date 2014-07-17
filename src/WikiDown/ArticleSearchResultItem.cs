using System;
using System.Collections.Generic;
using System.Linq;

using Raven.Imports.Newtonsoft.Json;
using WikiDown.RavenDb.Indexes;

namespace WikiDown
{
    public class ArticleSearchResultItem
    {
        //public string Id { get; set; }

        public string Highlighting { get; set; }

        public string RedirectToSlug { get; set; }

        public string Slug { get; set; }

        public string Title
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Slug) ? ArticleSlugUtility.Decode(this.Slug) : null;
            }
        }

        [JsonProperty]
        private IEnumerable<string> Highlightings
        {
            set
            {
                if (value != null && value.Any())
                {
                    this.Highlighting = value.FirstOrDefault();
                }
            }
        }

        [JsonProperty]
        private string TextContent
        {
            set
            {
                if (string.IsNullOrWhiteSpace(this.Highlighting))
                {
                    this.Highlighting = GetFallbackHighlightings(value);
                }
            }
        }

        private static string GetFallbackHighlightings(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            int length = Math.Min(text.Length, SearchArticlesIndex.HighlightFragmentLength);

            string textFragment = text.Substring(0, length);
            return textFragment;
        }
    }
}