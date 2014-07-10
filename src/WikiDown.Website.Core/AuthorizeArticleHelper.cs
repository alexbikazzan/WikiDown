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

        public static bool GetIsAuthorized(
            string articleIdParamName,
            AuthorizeType type,
            RequestContext requestContext,
            IDictionary<string, object> actionArguments)
        {
            string slugValue = GetSlugValue(articleIdParamName, actionArguments);

            var documentStore = DocumentStoreAppInstance.Get(requestContext.HttpContext.Application);
            var repository = RepositoryRequestInstance.Get(requestContext, documentStore);

            return GetIsAuthorizedInternal(slugValue, repository, type, requestContext.HttpContext.User);
        }

        public static bool GetIsAuthorized(
            string articleIdParamName,
            AuthorizeType type,
            HttpRequestContext requestContext,
            IDictionary<string, object> actionArguments)
        {
            string slugValue = GetSlugValue(articleIdParamName, actionArguments);

            var repository = RepositoryRequestInstance.Get(requestContext);

            return GetIsAuthorizedInternal(slugValue, repository, type, requestContext.Principal);
        }

        private static bool GetIsAuthorizedInternal(
            string slugValue,
            Repository repository,
            AuthorizeType type,
            IPrincipal principal)
        {
            var articleId = new ArticleId(slugValue ?? string.Empty);
            var articleAccess = (articleId.HasValue) ? repository.GetArticleAccess(articleId) : null;
            if (articleAccess == null)
            {
                return true;
            }

            throw new NotImplementedException();

            switch (type)
            {
                case AuthorizeType.CanRead:
                    break;
                case AuthorizeType.CanEdit:
                    break;
                case AuthorizeType.CanAdmin:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private static string GetSlugValue(string articleIdParamName, IDictionary<string, object> actionArguments)
        {
            object value;
            actionArguments.TryGetValue(articleIdParamName, out value);

            return Convert.ToString(value);
        }
    }
}