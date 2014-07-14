using System;
using System.Web.Mvc;

using WikiDown.Security;

namespace WikiDown.Website
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeArticleAttribute : ActionFilterAttribute
    {
        internal const string DefaultArticleIdParamName = "slug";

        private readonly ArticleAccessType accessType;

        public AuthorizeArticleAttribute(ArticleAccessType accessType)
        {
            this.accessType = accessType;
            this.ParamName = DefaultArticleIdParamName;
        }

        public string ParamName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            AuthorizeArticleHelper.EnsureIsAuthorized(
                this.ParamName,
                this.accessType,
                filterContext.RequestContext,
                filterContext.ActionParameters);
        }
    }
}