using System;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace WikiDown.Website
{
    public class AuthorizeArticleAttribute : ActionFilterAttribute
    {
        private const int ForbiddenStatusCode = (int)HttpStatusCode.Forbidden;

        private readonly string articleIdParamName;

        private readonly AuthorizeType type;

        public AuthorizeArticleAttribute(AuthorizeType type)
            : this(type, AuthorizeArticleHelper.DefaultArticleIdParamName)
        {
        }

        public AuthorizeArticleAttribute(AuthorizeType type, string articleIdParamName)
        {
            this.type = type;
            this.articleIdParamName = articleIdParamName;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            bool isAuthorized = AuthorizeArticleHelper.GetIsAuthorized(
                this.articleIdParamName,
                this.type,
                filterContext.RequestContext,
                filterContext.ActionParameters);
            if (!isAuthorized)
            {
                throw new HttpException(ForbiddenStatusCode, "Forbidden");
            }
        }
    }
}