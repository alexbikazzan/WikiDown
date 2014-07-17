using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Raven.Imports.Newtonsoft.Json;
using WikiDown.Security;

namespace WikiDown
{
    [DebuggerDisplay("Id={Id}, Title={Title}")]
    public class Article
    {
        private ArticleAccess articleAccess;

        public Article(ArticleId articleId, IEnumerable<string> tags = null, ArticleAccess articleAccess = null)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            this.Slug = articleId.Slug;

            this.Tags = tags ?? Enumerable.Empty<string>();
            this.ArticleAccess = articleAccess ?? ArticleAccess.Default();
        }

        [JsonConstructor]
        private Article()
        {
        }

        public string ActiveRevisionId { get; set; }

        public ArticleAccess ArticleAccess
        {
            get
            {
                return this.articleAccess ?? ArticleAccess.Default();
            }
            set
            {
                this.articleAccess = value;
            }
        }

        public string Id { get; set; }

        public string Slug { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string Title
        {
            get
            {
                return (this.Slug != null) ? ArticleSlugUtility.Decode(this.Slug) : null;
            }
        }
    }
}