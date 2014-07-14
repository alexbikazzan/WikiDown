using System;
using System.Web.Mvc;

using AspNetSeo;
using Microsoft.AspNet.Identity;
using Raven.Client;
using WikiDown.Website.Security;

namespace WikiDown.Website.Controllers
{
    public abstract class WikiDownControllerBase : SeoControllerBase
    {
        protected readonly IDocumentStore DocumentStore;

        private readonly Lazy<UserManager<WikiDownUser>> userManagerLazy;

        protected WikiDownControllerBase(IDocumentStore documentStore = null)
        {
            this.DocumentStore = documentStore ?? MvcApplication.DocumentStore;

            this.userManagerLazy = UserManagerHelper.GetLazy(this.DocumentStore);
        }

        protected Repository CurrentRepository
        {
            get
            {
                return RepositoryRequestInstance.Get(this.Request.RequestContext, this.DocumentStore);
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

        protected override void Dispose(bool disposing)
        {
            RepositoryRequestInstance.TryDispose(this.Request.RequestContext);

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