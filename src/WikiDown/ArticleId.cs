using System;
using System.Diagnostics;

namespace WikiDown
{
    [DebuggerDisplay("Id={Id}, Slug={Slug}, Title={Title}")]
    public class ArticleId
    {
        private static readonly string ArticleIdPrefix = (typeof(Article).Name.ToLowerInvariant() + "/");

        private readonly Lazy<bool> hasValueLazy;

        private readonly Lazy<string> idLazy;

        private readonly Lazy<string> slugLazy;

        private readonly Lazy<string> titleLazy;

        public ArticleId(string articleIdOrSlugOrTitle)
        {
            if (articleIdOrSlugOrTitle == null)
            {
                throw new ArgumentNullException("articleIdOrSlugOrTitle");
            }

            string articleIdOrSlug = ArticleSlugUtility.Decode(articleIdOrSlugOrTitle);

            bool isId = GetIsId(articleIdOrSlug);
            if (isId)
            {
                this.idLazy = new Lazy<string>(() => articleIdOrSlug);

                this.slugLazy = new Lazy<string>(() => GetArticleSlug(this.idLazy.Value));
            }
            else
            {
                this.slugLazy = new Lazy<string>(() => ArticleSlugUtility.Encode(articleIdOrSlug));

                this.idLazy = new Lazy<string>(() => CreateArticleId(this.slugLazy.Value));
            }

            this.titleLazy = new Lazy<string>(() => ArticleSlugUtility.Decode(this.slugLazy.Value));

            this.hasValueLazy =
                new Lazy<bool>(() => (!string.IsNullOrWhiteSpace(this.Id) || !string.IsNullOrWhiteSpace(this.Slug)));
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
            return new ArticleId(articleId);
        }

        public static string GetArticleSlug(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            return CreateArticleId(article.Id);
        }

        public static string GetArticleSlug(string articleId)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            int slashIndex = articleId.LastIndexOf('/');
            string articleSlug = (slashIndex >= 0) ? articleId.Substring(slashIndex + 1) : articleId;

            return ArticleSlugUtility.Encode(articleSlug);
        }

        public static string CreateArticleId(Article article)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            var articleId = new ArticleId(article.Title);
            return CreateArticleId(articleId);
        }

        public static string CreateArticleId(ArticleId articleId)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            return string.Format("{0}{1}", ArticleIdPrefix, articleId.Slug);
        }

        public static string CreateArticleRevisionId(ArticleRevision articleRevision)
        {
            if (articleRevision == null)
            {
                throw new ArgumentNullException("articleRevision");
            }

            return CreateArticleRevisionId(articleRevision.ArticleId, articleRevision.CreatedAt);
        }

        public static string CreateArticleRevisionId(ArticleId articleId, ArticleRevisionDate articleRevisionDate)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }
            if (articleRevisionDate == null)
            {
                throw new ArgumentNullException("articleRevisionDate");
            }

            return CreateArticleRevisionId(articleId, articleRevisionDate.DateTime);
        }

        public static string CreateArticleRevisionId(ArticleId articleId, DateTime articleRevisionDate)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }
            if (articleRevisionDate == DateTime.MinValue)
            {
                throw new ArgumentOutOfRangeException("articleRevisionDate");
            }

            string createdAtFormatted = articleRevisionDate.ToString(ArticleRevision.IdDateTimeFormat);
            return string.Format("{0}/{1}", articleId.Id, createdAtFormatted);
        }

        private static bool GetIsId(string articleIdOrSlug)
        {
            return (articleIdOrSlug != null) && articleIdOrSlug.StartsWith(ArticleIdPrefix);
        }
    }
}