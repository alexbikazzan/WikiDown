using System;
using System.Web;
using System.Web.Mvc;

namespace WikiDown.Website
{
    public class HandleExceptionsAttribute : HandleErrorAttribute //, IExceptionHandler
    {
        public override void OnException(ExceptionContext filterContext)
        {
            /*if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (filterContext.IsChildAction)
            {
                return;
            }

            if (filterContext.ExceptionHandled)
            {
                return;
            }

            var exception = filterContext.Exception;

            var httpException = new HttpException(null, exception);
            var httpExceptionHttpCode = httpException.GetHttpCode();
            if (httpExceptionHttpCode != 500)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(exception))
            {
                return;
            }

            var handledExceptionResult = HandleExceptions(filterContext, exception);

            filterContext.ExceptionHandled = (handledExceptionResult != null);
            filterContext.Result = handledExceptionResult;

            if (filterContext.ExceptionHandled)
            {
                return;
            }*/

            base.OnException(filterContext);
        }

        private static ActionResult HandleExceptions(ControllerContext filterContext, Exception exception)
        {
            var urlHelper = new UrlHelper(filterContext.RequestContext);

            var articleIdNotEnsuredException = exception as ArticleIdNotEnsuredException;
            if (articleIdNotEnsuredException != null)
            {
                string articleInfotUrl = urlHelper.WikiArticle(articleIdNotEnsuredException.EnsuredSlug);
                return new RedirectResult(articleInfotUrl);
            }

            return null;
        }

        //public Task HandleAsync(ExceptionHandlerContext context, System.Threading.CancellationToken cancellationToken)
        //{

        //    return Task.FromResult<object>(null);
        //}

        //private bool ShouldHandle(ExceptionHandlerContext context)
        //{
        //    var exceptionContext = context.ExceptionContext;
        //    if (exceptionContext == null)
        //    {
        //        throw new NullReferenceException("ExceptionContext is null.");
        //    }

        //    var catchBlock = exceptionContext.CatchBlock;

        //    return catchBlock.IsTopLevel;
        //}
    }
}