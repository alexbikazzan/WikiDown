using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using WikiDown.Security;

namespace WikiDown.Website
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeArticleApiAttribute : ActionFilterAttribute
    {
        private readonly ArticleAccessType accessType;

        public AuthorizeArticleApiAttribute(ArticleAccessType accessType)
        {
            this.accessType = accessType;
            this.ParamName = AuthorizeArticleAttribute.DefaultArticleIdParamName;
        }

        public string ParamName { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException("actionContext");
            }

            AuthorizeArticleHelper.EnsureIsAuthorized(
                this.ParamName,
                this.accessType,
                actionContext.RequestContext,
                actionContext.ActionArguments);
        }
    }
}