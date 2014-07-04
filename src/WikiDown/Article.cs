using System;
using System.Diagnostics;

namespace WikiDown
{
    [DebuggerDisplay("Id={Id}, Title={Title}")]
    public class Article
    {
        public Article()
        {
        }

        public Article(ArticleId articleId, string metaKeywords = null)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            this.Title = articleId.Title;
            this.MetaKeywords = metaKeywords;

            this.CreatedAt = DateTime.UtcNow;
        }

        public string ActiveRevisionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Id { get; set; }

        public bool IsDeleted { get; set; }

        public string MetaKeywords { get; set; }

        public string Title { get; private set; }
    }
}