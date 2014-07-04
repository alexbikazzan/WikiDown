using System.Web.Mvc;

namespace WikiDown.Website
{
    public static class UrlHelperHttpExtensions
    {
        public static string ApiArticleRevision(
            this UrlHelper urlHelper,
            ArticleId articleId,
            ArticleRevisionDate revisionDate)
        {
            return urlHelper.HttpRouteUrl(
                ApiRouteNames.ArticleRevisions,
                new { articleId = articleId.Slug, articleRevisionDate = revisionDate });
        }

        public static string ApiArticleRevisions(this UrlHelper urlHelper, ArticleId articleId)
        {
            return urlHelper.HttpRouteUrl(ApiRouteNames.ArticleRevisions, new { articleId = articleId.Slug });
        }

        public static string ApiArticleRevisionPreview(
            this UrlHelper urlHelper,
            ArticleId articleId,
            ArticleRevisionDate revisionDate)
        {
            return urlHelper.HttpRouteUrl(
                ApiRouteNames.ArticleRevisionPreview,
                new { articleId = articleId.Slug, articleRevisionDate = revisionDate });
        }
    }
}