using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

using AspNetSeo;
using Microsoft.AspNet.Identity;
using Raven.Client;
using WikiDown.Security;
using WikiDown.Website.Security;

namespace WikiDown.Website.Controllers
{
    public abstract class WikiDownControllerBase : SeoControllerBase
    {
        protected const int UnauthorizedHttpCode = (int)HttpStatusCode.Unauthorized;

        private readonly Lazy<ArticleAccessManager> articleAccessManagerLazy;

        private readonly Lazy<Repository> currentRepositoryLazy;

        private readonly Lazy<UserManager<WikiDownUser>> userManagerLazy;

        protected WikiDownControllerBase(IDocumentStore documentStore = null)
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

        protected RedirectResult Redirect(Func<UrlHelper, string> urlFactory, bool permanent = false)
        {
            string url = urlFactory(this.Url);
            return permanent ? this.RedirectPermanent(url) : this.Redirect(url);
        }

        protected void EnsureCanReadArticle(ArticleId articleId, IPrincipal principal)
        {
            var articleAccess = this.CurrentRepository.GetArticleAccess(articleId);

            bool canRead = this.ArticleAccessManager.GetCanRead(articleAccess, principal);
            if (!canRead)
            {
                throw new HttpException(UnauthorizedHttpCode, "Read Unauthorized");
            }

            this.SetSeoValues(articleAccess);
        }

        protected void EnsureCanEditArticle(ArticleId articleId, IPrincipal principal)
        {
            var articleAccess = this.CurrentRepository.GetArticleAccess(articleId);

            bool canEdit = this.ArticleAccessManager.GetCanEdit(articleAccess, principal);
            if (!canEdit)
            {
                throw new HttpException(UnauthorizedHttpCode, "Edit Unauthorized");
            }

            this.SetSeoValues(articleAccess);
        }

        protected void EnsureCanAdminArticle(ArticleId articleId, IPrincipal principal)
        {
            var articleAccess = this.CurrentRepository.GetArticleAccess(articleId);

            bool canAdmin = this.ArticleAccessManager.GetCanAdmin(articleAccess, principal);
            if (!canAdmin)
            {
                throw new HttpException(UnauthorizedHttpCode, "Admin Unauthorized");
            }

            this.SetSeoValues(articleAccess);
        }

        protected override void Dispose(bool disposing)
        {
            TryDisposeLazy(this.currentRepositoryLazy);

            TryDisposeLazy(this.userManagerLazy);

            base.Dispose(disposing);
        }

        private void SetSeoValues(ArticleAccess articleAccess)
        {
            if (articleAccess == null || articleAccess.CanRead > ArticleAccessLevel.Anonymous)
            {
                this.Seo.MetaNoIndex = true;
            }
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