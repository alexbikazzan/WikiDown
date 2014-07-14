using System;
using System.Web.Mvc;

using AspNetSeo;
using Raven.Client;
using WikiDown.Website.ViewModels;

namespace WikiDown.Website.Controllers
{
    public class WikiController : WikiDownControllerBase
    {
        public WikiController()
            : this(null)
        {
        }

        public WikiController(IDocumentStore documentStore)
            : base(documentStore)
        {
        }

        [BodyClass("wiki article index")]
        [Route("wiki", Name = RouteNames.WikiIndex)]
        public ActionResult Index()
        {
            var model = new WikiArticleViewModel(this.Request.RequestContext, string.Empty);
            return this.View(model);
        }

        [SeoTitle("All Articles")]
        [BodyClass("wiki list")]
        [Route("list", Name = RouteNames.WikiList)]
        public ActionResult List()
        {
            var model = new WikiListViewModel(this.CurrentRepository);
            return this.View(model);
        }

        [EnsureSlug(RouteName = RouteNames.WikiArticle)]
        [BodyClass("wiki article")]
        [Route("wiki/{slug}/{revisionDate?}", Name = RouteNames.WikiArticle)]
        public ActionResult Article(string slug, ArticleRevisionDate revisionDate = null, bool redirect = true)
        {
            var model = new WikiArticleViewModel(this.Request.RequestContext, slug, revisionDate, redirect);
            this.Seo.CanonicalUrl = this.GetArticleCanonicalUrl(model);

            return this.View(model);
        }

        [HttpGet]
        [BodyClass("wiki edit")]
        [EnsureSlug(RouteName = RouteNames.WikiArticleEdit)]
        [SeoMetaNoIndex]
        [Route("edit/{slug}/{revisionDate?}", Name = RouteNames.WikiArticleEdit)]
        public ActionResult Edit(string slug, ArticleRevisionDate revisionDate = null)
        {
            var model = new WikiArticleEditViewModel(this.Request.RequestContext, slug, revisionDate);
            return this.View(model);
        }

        [BodyClass("wiki info")]
        [EnsureSlug(RouteName = RouteNames.WikiArticleInfo)]
        [Route("info/{slug}", Name = RouteNames.WikiArticleInfo)]
        public ActionResult Info(string slug)
        {
            try
            {
                var model = new WikiArticleInfoViewModel(this.Request.RequestContext, slug, this.UserManager);
                return this.View(model);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                if (ex.ParamName == "articleId")
                {
                    string redirectUrl = this.Url.WikiArticle(slug);
                    return this.Redirect(redirectUrl);
                }

                throw;
            }
        }

        [HttpPost]
        [Route("search", Name = RouteNames.WikiSearch)]
        public ActionResult Search(string search)
        {
            return this.Redirect(url => url.WikiArticle(search));
        }

        private string GetArticleCanonicalUrl(WikiArticleViewModel model)
        {
            string canonicalUrl = null;

            if (model.ShouldRedirect)
            {
                canonicalUrl = this.Url.WikiArticle(model.ArticleRedirectTo);
            }
            else if (model.HasArticle)
            {
                canonicalUrl = this.Url.WikiArticle(model.ArticleId);
            }

            return canonicalUrl;
        }
    }
}