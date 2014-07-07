namespace WikiDown.Security
{
    public static class RepositorySecurityExtensions
    {
        public static bool DeleteArticleAccess(this Repository repository, ArticleId articleId)
        {
            var articleAccess = repository.GetArticleAccess(articleId);
            if (articleAccess != null)
            {
                repository.CurrentSession.Delete(articleAccess);

                repository.CurrentSession.SaveChanges();
            }

            return (articleAccess != null);
        }

        public static ArticleAccess GetArticleAccess(this Repository repository, ArticleId articleId)
        {
            var articleAccessId = (articleId != null && articleId.HasValue)
                                      ? SecurityIdUtility.GetArticleAccessId(articleId)
                                      : null;

            return !string.IsNullOrWhiteSpace(articleAccessId)
                       ? repository.CurrentSession.Load<ArticleAccess>(articleAccessId)
                       : null;
        }

        public static void SaveArticleAccess(this Repository repository, ArticleAccess articleAccess)
        {
            repository.CurrentSession.Store(articleAccess);

            repository.CurrentSession.SaveChanges();
        }
    }
}