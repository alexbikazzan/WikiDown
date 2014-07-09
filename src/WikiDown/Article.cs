using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WikiDown
{
    [DebuggerDisplay("Id={Id}, Title={Title}")]
    public class Article
    {
        public Article()
        {
        }

        public Article(ArticleId articleId, IEnumerable<string> tags = null)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            this.Title = articleId.Title;
            this.Tags = tags ?? Enumerable.Empty<string>();

            this.CreatedAt = DateTime.UtcNow;
        }

        public string ActiveRevisionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Id { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string Title { get; private set; }
    }
}