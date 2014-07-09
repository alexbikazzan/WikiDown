using System;
using System.Diagnostics;

namespace WikiDown
{
    [DebuggerDisplay("Id={Id}, Slug={Slug}, Title={Title}")]
    public class ArticleId
    {
        private readonly Lazy<bool> hasValueLazy;

        private readonly Lazy<string> idLazy;

        private readonly Lazy<string> originalSlugLazy;

        private readonly Lazy<string> slugLazy;

        private readonly Lazy<string> titleLazy;

        public ArticleId(string articleIdOrSlugOrTitle)
        {
            if (articleIdOrSlugOrTitle == null)
            {
                throw new ArgumentNullException("articleIdOrSlugOrTitle");
            }

            this.hasValueLazy = new Lazy<bool>(() => (!string.IsNullOrWhiteSpace(articleIdOrSlugOrTitle)));

            string articleIdOrSlug = ArticleSlugUtility.Decode(articleIdOrSlugOrTitle);

            bool isId = GetIsId(articleIdOrSlug);
            if (isId)
            {
                this.originalSlugLazy = this.slugLazy;
                this.slugLazy = new Lazy<string>(() => GetArticleSlug(articleIdOrSlug));
            }
            else
            {
                this.originalSlugLazy = new Lazy<string>(() => articleIdOrSlugOrTitle);
                this.slugLazy = new Lazy<string>(() => ArticleSlugUtility.Encode(articleIdOrSlug));
            }

            this.idLazy = new Lazy<string>(() => IdUtility.CreateArticleId(this.slugLazy.Value));

            this.titleLazy = new Lazy<string>(() => ArticleSlugUtility.Decode(this.slugLazy.Value));
        }

        public bool HasValue
        {
            get
            {
                return this.hasValueLazy.Value;
            }
        }

        public string Id
        {
            get
            {
                return this.idLazy.Value;
            }
        }

        public string OriginalSlug
        {
            get
            {
                return this.originalSlugLazy.Value;
            }
        }

        public string Slug
        {
            get
            {
                return this.slugLazy.Value;
            }
        }

        public string Title
        {
            get
            {
                return this.titleLazy.Value;
            }
        }

        public override string ToString()
        {
            return this.Id ?? string.Empty;
        }

        public static implicit operator ArticleId(string articleId)
        {
            return new ArticleId(articleId ?? string.Empty);
        }

        private static string GetArticleSlug(string articleId)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            int slashIndex = articleId.LastIndexOf('/');
            string articleSlug = (slashIndex >= 0) ? articleId.Substring(slashIndex + 1) : articleId;

            return ArticleSlugUtility.Encode(articleSlug);
        }

        private static bool GetIsId(string articleIdOrSlug)
        {
            return (articleIdOrSlug != null) && articleIdOrSlug.StartsWith(IdUtility.ArticleIdPrefix);
        }
    }
}