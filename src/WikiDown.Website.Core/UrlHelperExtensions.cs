using System;
using System.Web.Mvc;

namespace WikiDown.Website
{
    public static class UrlHelperExtensions
    {
        public static string Absolute(this UrlHelper urlHelper, string url)
        {
            if (string.IsNullOrWhiteSpace(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return url;
            }

            var requestUrl = urlHelper.RequestContext.HttpContext.Request.Url;
            string domain = (requestUrl != null) ? requestUrl.Host.Trim('/') : null;

            var contentUrl = urlHelper.Content(url).TrimStart('/');

            return string.Format("http://{0}/{1}", domain, contentUrl);
        }

        public static string Empty(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl(RouteNames.Empty);
        }

        public static string AccountAdmin(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl(RouteNames.AccountAdmin); //, new { Area = AreaNames.AccountAdmin });
        }

        public static string Login(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl(RouteNames.Login);
        }

        public static string Logout(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl(RouteNames.Logout);
        }

        public static string SiteAbout(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl(RouteNames.Site, new { action = "About" });
        }

        public static string WikiArticle(
            this UrlHelper urlHelper,
            ArticleId articleId,
            ArticleRevisionDate revisionDate = null,
            bool redirect = true)
        {
            var noRedirectValue = redirect ? null : "0";

            return urlHelper.RouteUrl(
                RouteNames.WikiArticle,
                new { slug = articleId.Slug, revisionDate, redirect = noRedirectValue });
        }

        public static string WikiArticleEdit(
            this UrlHelper urlHelper,
            ArticleId articleId,
            ArticleRevisionDate revisionDate = null)
        {
            return urlHelper.RouteUrl(RouteNames.WikiArticleEdit, new { slug = articleId.Slug, revisionDate });
        }

        public static string WikiArticleInfo(this UrlHelper urlHelper, ArticleId articleId)
        {
            return urlHelper.RouteUrl(RouteNames.WikiArticleInfo, new { slug = articleId.Slug });
        }

        public static string WikiDeleteArticleRevision(
            this UrlHelper urlHelper,
            ArticleId articleId,
            ArticleRevisionDate revisionDate)
        {
            return urlHelper.RouteUrl(RouteNames.WikiArticleRevisionDelete, new { slug = articleId.Slug, revisionDate });
        }

        public static string WikiList(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl(RouteNames.WikiList);
        }

        public static string WikiSearch(this UrlHelper urlHelper, string search)
        {
            return urlHelper.RouteUrl(RouteNames.WikiSearch, new { q = search });
        }
    }
}