using System;

namespace WikiDown.Website
{
    public class ArticleNotFoundException : Exception
    {
        public ArticleNotFoundException(ArticleId articleId)
            : this(articleId, GetDefaultMessage(articleId))
        {
        }

        public ArticleNotFoundException(ArticleId articleId, string message)
            : base(message)
        {
            this.ArticleId = articleId;
        }

        public ArticleNotFoundException(ArticleId articleId, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ArticleId = articleId;
        }

        public ArticleId ArticleId { get; private set; }

        private static string GetDefaultMessage(ArticleId articleId)
        {
            return (articleId != null)
                       ? string.Format("Cannot find article with ID '{0}'.", articleId.Slug)
                       : "Cannot find article.";
        }
    }
}