using System;
using System.Web.Http;

using Microsoft.AspNet.Identity;
using Raven.Client;
using WikiDown.Website.Security;

namespace WikiDown.Website.Controllers.Api
{
    public abstract class WikiDownApiControllerBase : ApiController
    {
        protected readonly IDocumentStore DocumentStore;

        private readonly Lazy<UserManager<WikiDownUser>> userManagerLazy;

        protected WikiDownApiControllerBase(IDocumentStore documentStore = null)
        {
            this.DocumentStore = documentStore ?? MvcApplication.DocumentStore;

            this.userManagerLazy = UserManagerHelper.GetLazy(this.DocumentStore);
        }

        protected Repository CurrentRepository
        {
            get
            {
                return RepositoryRequestInstance.Get(this.RequestContext, this.DocumentStore);
            }
        }

        protected UserManager<WikiDownUser> UserManager
        {
            get
            {
                return this.userManagerLazy.Value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            RepositoryRequestInstance.TryDispose(this.RequestContext);

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