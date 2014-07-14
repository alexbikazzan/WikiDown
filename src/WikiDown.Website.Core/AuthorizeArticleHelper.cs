using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Http.Controllers;
using System.Web.Routing;

using WikiDown.Security;

namespace WikiDown.Website
{
    internal static class AuthorizeArticleHelper
    {
        public const string DefaultArticleIdParamName = "slug";

        public static void EnsureIsAuthorized(
            string articleIdParamName,
            ArticleAccessType accessType,
            RequestContext requestContext,
            IDictionary<string, object> actionArguments)
        {
            string slugValue = GetSlugValue(articleIdParamName, actionArguments);

            var documentStore = DocumentStoreAppInstance.Get(requestContext.HttpContext.Application);
            var repository = RepositoryRequestInstance.Get(requestContext, documentStore);

            EnsureIsAuthorizedInternal(slugValue, repository, accessType, requestContext.HttpContext.User);
        }

        public static void EnsureIsAuthorized(
            string articleIdParamName,
            ArticleAccessType accessType,
            HttpRequestContext requestContext,
            IDictionary<string, object> actionArguments)
        {
            string slugValue = GetSlugValue(articleIdParamName, actionArguments);

            var repository = RepositoryRequestInstance.Get(requestContext);

            EnsureIsAuthorizedInternal(slugValue, repository, accessType, requestContext.Principal);
        }

        private static void EnsureIsAuthorizedInternal(
            string slugValue,
            Repository repository,
            ArticleAccessType accessType,
            IPrincipal principal)
        {
            var articleId = new ArticleId(slugValue ?? string.Empty);
            var article = (articleId.HasValue) ? repository.GetArticle(articleId) : null;
            if (article == null)
            {
                return;
            }

            article.EnsureCanAccess(principal, accessType);
        }

        private static string GetSlugValue(string articleIdParamName, IDictionary<string, object> actionArguments)
        {
            object value;
            actionArguments.TryGetValue(articleIdParamName, out value);

            return Convert.ToString(value);
        }
    }
}