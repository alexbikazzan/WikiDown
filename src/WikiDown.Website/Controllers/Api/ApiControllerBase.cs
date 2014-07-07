using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using Raven.Client;
using WikiDown.Security;
using WikiDown.Website.Security;

namespace WikiDown.Website.Controllers.Api
{
    public abstract class ApiControllerBase : ApiController
    {
        internal const int UnauthorizedHttpCode = (int)HttpStatusCode.Unauthorized;

        private readonly Lazy<ArticleAccessManager> articleAccessManagerLazy;

        private readonly Lazy<Repository> currentRepositoryLazy;

        private readonly Lazy<UserManager<WikiDownUser>> userManagerLazy;

        protected ApiControllerBase(IDocumentStore documentStore = null)
        {
            documentStore = documentStore ?? MvcApplication.DocumentStore;

            this.articleAccessManagerLazy =
                new Lazy<ArticleAccessManager>(() => new ArticleAccessManager(this.currentRepositoryLazy.Value));

            this.currentRepositoryLazy = new Lazy<Repository>(() => new Repository(documentStore));

            this.userManagerLazy = UserManagerHelper.GetLazy(documentStore);
        }

        public ArticleAccessManager ArticleAccessManager
        {
            get
            {
                return this.articleAccessManagerLazy.Value;
            }
        }

        public Repository CurrentRepository
        {
            get
            {
                return this.currentRepositoryLazy.Value;
            }
        }

        public UserManager<WikiDownUser> UserManager
        {
            get
            {
                return this.userManagerLazy.Value;
            }
        }

        public void ValidateCanReadArticle(ArticleId articleId, IPrincipal principal)
        {
            bool canRead = this.ArticleAccessManager.GetCanRead(articleId, principal);
            if (!canRead)
            {
                throw new HttpException(UnauthorizedHttpCode, "Read Unauthorized");
            }
        }

        public void ValidateCanEditArticle(ArticleId articleId, IPrincipal principal)
        {
            bool canEdit = this.ArticleAccessManager.GetCanEdit(articleId, principal);
            if (!canEdit)
            {
                throw new HttpException(UnauthorizedHttpCode, "Edit Unauthorized");
            }
        }

        public void ValidateCanAdminArticle(ArticleId articleId, IPrincipal principal)
        {
            bool canAdmin = this.ArticleAccessManager.GetCanAdmin(articleId, principal);
            if (!canAdmin)
            {
                throw new HttpException(UnauthorizedHttpCode, "Admin Unauthorized");
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