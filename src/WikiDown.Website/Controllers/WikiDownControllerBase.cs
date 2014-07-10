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
        private readonly IDocumentStore documentStore;

        protected const int UnauthorizedHttpCode = (int)HttpStatusCode.Unauthorized;

        private readonly Lazy<ArticleAccessManager> articleAccessManagerLazy;

        private readonly Lazy<UserManager<WikiDownUser>> userManagerLazy;

        protected WikiDownControllerBase(IDocumentStore documentStore = null)
        {
            this.documentStore = documentStore ?? MvcApplication.DocumentStore;

            this.articleAccessManagerLazy =
                new Lazy<ArticleAccessManager>(() => new ArticleAccessManager(this.CurrentRepository));


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
                return RepositoryRequestInstance.Get(this.Request.RequestContext, this.documentStore);
                //return this.currentRepositoryLazy.Value;
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
            RepositoryRequestInstance.TryDispose(this.Request.RequestContext);

            TryDisposeLazy(this.userManagerLazy);

            base.Dispose(disposing);
        }

        private void SetSeoValues(ArticleAccess articleAccess)
        {
            if (articleAccess == null || articleAccess.CanRead > ArticleAccessRole.Anonymous)
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