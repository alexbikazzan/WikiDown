using System;

namespace WikiDown
{
    public static class IdUtility
    {
        internal static readonly string ArticleIdPrefix = (typeof(Article).Name + "/");

        internal static readonly string ArticleRedirectIdPrefix = (typeof(ArticleRedirect).Name + "/");

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

            return String.Format("{0}{1}", ArticleIdPrefix, articleId.Slug);
        }

        public static string CreateArticleRedirectId(ArticleRedirect articleRedirect)
        {
            if (articleRedirect == null)
            {
                throw new ArgumentNullException("articleRedirect");
            }

            return CreateArticleRedirectId(articleRedirect.OriginalArticleSlug);
        }

        public static string CreateArticleRedirectId(ArticleId originalArticleId)
        {
            if (originalArticleId == null)
            {
                throw new ArgumentNullException("originalArticleId");
            }

            return String.Format("{0}{1}", ArticleRedirectIdPrefix, originalArticleId.Slug);
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

            string createdAtFormatted = articleRevisionDate.ToString(ArticleRevisionDate.IdDateTimeFormat);
            return String.Format("{0}/{1}", articleId.Id, createdAtFormatted);
        }
    }
}