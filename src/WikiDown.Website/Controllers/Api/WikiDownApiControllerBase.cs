using System;
using System.Net;
using System.Security.Principal;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using Raven.Client;
using WikiDown.Security;
using WikiDown.Website.Security;

namespace WikiDown.Website.Controllers.Api
{
    public abstract class WikiDownApiControllerBase : ApiController
    {
        internal const int UnauthorizedHttpCode = (int)HttpStatusCode.Unauthorized;

        private readonly Lazy<ArticleAccessManager> articleAccessManagerLazy;

        private readonly Lazy<Repository> currentRepositoryLazy;

        private readonly Lazy<UserManager<WikiDownUser>> userManagerLazy;

        protected WikiDownApiControllerBase(IDocumentStore documentStore = null)
        {
            documentStore = documentStore ?? MvcApplication.DocumentStore;

            this.articleAccessManagerLazy =
                new Lazy<ArticleAccessManager>(() => new ArticleAccessManager(this.currentRepositoryLazy.Value));

            this.currentRepositoryLazy = new Lazy<Repository>(() => new Repository(documentStore));

            this.userManagerLazy = UserManagerHelper.GetLazy(documentStore);
        }

        protected ArticleAccessManager ArticleAccessManager
        {
            get
            {
                return this.articleAccessManagerLazy.Value;
            }
        }

        protected Repository CurrentRepository
        {
            get
            {
                return this.currentRepositoryLazy.Value;
            }
        }

        protected UserManager<WikiDownUser> UserManager
        {
            get
            {
                return this.userManagerLazy.Value;
            }
        }

        protected void EnsureCanReadArticle(ArticleId articleId, IPrincipal principal)
        {
            bool canRead = this.ArticleAccessManager.GetCanRead(articleId, principal);
            if (!canRead)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
        }

        protected void EnsureCanEditArticle(ArticleId articleId, IPrincipal principal)
        {
            bool canEdit = this.ArticleAccessManager.GetCanEdit(articleId, principal);
            if (!canEdit)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
        }

        protected void EnsureCanAdminArticle(ArticleId articleId, IPrincipal principal)
        {
            bool canAdmin = this.ArticleAccessManager.GetCanAdmin(articleId, principal);
            if (!canAdmin)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
        }

        protected override void Dispose(bool disposing)
        {
            TryDisposeLazy(this.currentRepositoryLazy);

            TryDisposeLazy(this.userManagerLazy);

            base.Dispose(disposing);
        }

        private static void TryDisposeLazy<TDisposable>(Lazy<TDisposable> lazyDisposable)
            where TDisposable : class, IDisposable
        {
            var value = lazyDisposable.IsValueCreated ? lazyDisposable.Value : null;
            if (value != null)
            {
                value.Dispose();
            }
        }
    }
}