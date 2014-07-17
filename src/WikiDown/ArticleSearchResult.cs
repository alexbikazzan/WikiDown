using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiDown
{
    public class ArticleSearchResult
    {
        public ArticleSearchResult(IEnumerable<ArticleSearchResultItem> items, IEnumerable<string> suggestions)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.Items = items.ToList();
            this.Suggestions = (suggestions ?? Enumerable.Empty<string>()).ToList();
        }

        public IReadOnlyCollection<ArticleSearchResultItem> Items { get; private set; }

        public IReadOnlyCollection<string> Suggestions { get; private set; }
    }
}