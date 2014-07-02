﻿using System;
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

        public static string SiteAbout(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl(RouteNames.Site, new { action = "About" });
        }

        public static string WikiArticle(
            this UrlHelper urlHelper,
            ArticleId articleId,
            ArticleRevisionDate revisionCreated = null)
        {
            return urlHelper.RouteUrl(RouteNames.WikiArticle, new { slug = articleId.Slug, revisionCreated });
        }

        public static string WikiArticleEdit(
            this UrlHelper urlHelper,
            ArticleId articleId,
            ArticleRevisionDate revisionCreated = null,
            bool noRedirect = false)
        {
            var noRedirectValue = noRedirect ? 1 : (int?)null;

            return urlHelper.RouteUrl(
                RouteNames.WikiArticleEdit,
                new { slug = articleId.Slug, revisionCreated, noRedirect = noRedirectValue });
        }

        public static string WikiArticleInfo(this UrlHelper urlHelper, ArticleId articleId)
        {
            return urlHelper.RouteUrl(RouteNames.WikiArticleInfo, new { slug = articleId.Slug });
        }

        public static string WikiList(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl(RouteNames.WikiList);
        }

        public static string WikiSearch(this UrlHelper urlHelper, string search)
        {
            return urlHelper.RouteUrl(RouteNames.WikiSearch, new { search });
        }
    }
}