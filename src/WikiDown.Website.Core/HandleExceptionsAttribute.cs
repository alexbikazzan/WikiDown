using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;

using MvcExceptionContext = System.Web.Mvc.ExceptionContext;
using WebApiExceptionContext = System.Web.Http.ExceptionHandling.ExceptionContext;

namespace WikiDown.Website
{
    public class HandleExceptionsAttribute : HandleErrorAttribute, IExceptionHandler
    {
        public override void OnException(MvcExceptionContext filterContext)
        {
            //if (filterContext == null)
            //{
            //    throw new ArgumentNullException("filterContext");
            //}
            //if (filterContext.IsChildAction)
            //{
            //    return;
            //}

            //if (filterContext.ExceptionHandled)
            //{
            //    return;
            //}

            //var exception = filterContext.Exception;

            //var httpException = new HttpException(null, exception);
            //var httpExceptionHttpCode = httpException.GetHttpCode();
            //if (httpExceptionHttpCode != 500)
            //{
            //    return;
            //}

            //if (!ExceptionType.IsInstanceOfType(exception))
            //{
            //    return;
            //}

            //var handledExceptionResult = HandleExceptions(filterContext, exception);

            //filterContext.ExceptionHandled = (handledExceptionResult != null);
            //filterContext.Result = handledExceptionResult;

            //if (filterContext.ExceptionHandled)
            //{
            //    return;
            //}

            base.OnException(filterContext);
        }

        private static ActionResult HandleExceptions(MvcExceptionContext filterContext, Exception exception)
        {
            //var urlHelper = new UrlHelper(filterContext.RequestContext);

            //var articleNotFoundException = exception as ArticleNotFoundException;
            //if (articleNotFoundException != null)
            //{
            //    var notFoundArticleId = new ArticleId(articleNotFoundException.Slug);
            //    string notFoundArticleTilte = ArticleSlugUtility.Decode(notFoundArticleId.Slug);

            //    var model = new KeyValuePair<string, string>(notFoundArticleId.Slug, notFoundArticleTilte);
            //    return new ViewResult
            //               {
            //                   ViewName = ViewNames.WikiArticleNotFound,
            //                   ViewData = new ViewDataDictionary(model)
            //               };
            //}

            //var articleHasRedirectException = exception as ArticleRedirectException;
            //if (articleHasRedirectException != null)
            //{
            //    string redirectUrl = urlHelper.WikiArticle(articleHasRedirectException.RedirectToArticleSlug);
            //    return new RedirectResult(redirectUrl);
            //}

            //var articleHasNoActiveRevisionException = exception as ArticleHasNoActiveRevisionException;
            //if (articleHasNoActiveRevisionException != null)
            //{
            //    string articleInfotUrl = urlHelper.WikiArticleInfo(articleHasNoActiveRevisionException.Slug);
            //    return new RedirectResult(articleInfotUrl);
            //}

            return null;
        }

        public Task HandleAsync(ExceptionHandlerContext context, System.Threading.CancellationToken cancellationToken)
        {

            return Task.FromResult<object>(null);
        }

        private bool ShouldHandle(ExceptionHandlerContext context)
        {
            var exceptionContext = context.ExceptionContext;
            if (exceptionContext == null)
            {
                throw new NullReferenceException("ExceptionContext is null.");
            }

            var catchBlock = exceptionContext.CatchBlock;

            return catchBlock.IsTopLevel;
        }
    }
}