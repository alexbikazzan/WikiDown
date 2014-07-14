using System;

namespace WikiDown.Security
{
    public class ArticleAccessException : Exception
    {
        public ArticleAccessException(ArticleId articleId)
            : this(articleId, GetDefaultMessage(articleId))
        {
        }

        public ArticleAccessException(ArticleId articleId, string message)
            : base(message)
        {
            this.ArticleId = articleId;
        }

        public ArticleAccessException(ArticleId articleId, string message, Exception innerException)
            : base(message, innerException)
        {
            this.ArticleId = articleId;
        }

        public ArticleId ArticleId { get; private set; }

        private static string GetDefaultMessage(ArticleId articleId)
        {
            return (articleId != null)
                       ? string.Format("Cannot access article with ID '{0}'.", articleId.Slug)
                       : "Cannot access article.";
        }
    }
}