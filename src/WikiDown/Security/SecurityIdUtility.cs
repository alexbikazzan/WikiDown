using System;

namespace WikiDown.Security
{
    public static class SecurityIdUtility
    {
        private static readonly string ArticleAccessIdSuffix = (typeof(ArticleAccess).Name.ToLowerInvariant());

        public static string GetArticleAccessId(ArticleAccess articleAccess)
        {
            if (articleAccess == null)
            {
                throw new ArgumentNullException("articleAccess");
            }

            return GetArticleAccessId(articleAccess.ArticleId);
        }

        public static string GetArticleAccessId(ArticleId articleId)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            return articleId.HasValue ? string.Format("{0}/{1}", articleId.Id, ArticleAccessIdSuffix) : null;
        }
    }
}