using System;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WikiDown.Website
{
    public class AuthorizeArticleApiAttribute : ActionFilterAttribute
    {
        private readonly string articleIdParamName;

        private readonly AuthorizeType type;

        public AuthorizeArticleApiAttribute(AuthorizeType type)
            : this(type, AuthorizeArticleHelper.DefaultArticleIdParamName)
        {
        }

        public AuthorizeArticleApiAttribute(AuthorizeType type, string articleIdParamName)
        {
            this.type = type;
            this.articleIdParamName = articleIdParamName;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            bool isAuthorized = AuthorizeArticleHelper.GetIsAuthorized(
                this.articleIdParamName,
                this.type,
                actionContext.RequestContext,
                actionContext.ActionArguments);
            if (!isAuthorized)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
        }
    }
}