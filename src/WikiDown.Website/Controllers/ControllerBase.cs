using System;
using System.Web.Mvc;

using AspNetSeo;

namespace WikiDown.Website.Controllers
{
    public abstract class ControllerBase : SeoControllerBase
    {
        private readonly Lazy<Repository> currentRepositoryLazy;

        protected ControllerBase(Repository repository = null)
        {
            this.currentRepositoryLazy =
                new Lazy<Repository>(() => repository ?? new Repository(MvcApplication.DocumentStore));
        }

        public Repository CurrentRepository
        {
            get
            {
                return this.currentRepositoryLazy.Value;
            }
        }

        public RedirectResult Redirect(Func<UrlHelper, string> urlFactory, bool permanent = false)
        {
            string url = urlFactory(this.Url);
            return permanent ? this.RedirectPermanent(url) : this.Redirect(url);
        }

        protected override void Dispose(bool disposing)
        {
            var repository = this.currentRepositoryLazy.IsValueCreated ? this.currentRepositoryLazy.Value : null;
            if (repository != null)
            {
                repository.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}