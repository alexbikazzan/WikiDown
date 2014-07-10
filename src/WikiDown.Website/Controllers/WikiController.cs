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
        public ActionResult Index() {
            var articleResult = this.CurrentRepository.GetArticleResult(string.Empty);

            var model = new WikiArticleViewModel(string.Empty, articleResult);
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

        [BodyClass("wiki article")]
        [EnsureSlug(RouteName = RouteNames.WikiArticle)]
        [Route("wiki/{slug}/{revisionDate?}", Name = RouteNames.WikiArticle)]
        public ActionResult Article(string slug, ArticleRevisionDate revisionDate = null, bool redirect = true)
        {
            this.EnsureCanReadArticle(slug, this.User);

            var articleResult = this.CurrentRepository.GetArticleResult(slug, revisionDate);

            this.Seo.CanonicalUrl = this.GetArticleCanonicalUrl(articleResult);

            var model = new WikiArticleViewModel(slug, articleResult, revisionDate, redirect);
            return this.View(model);
        }

        [HttpGet]
        [BodyClass("wiki edit")]
        [EnsureSlug(RouteName = RouteNames.WikiArticleEdit)]
        [SeoMetaNoIndex]
        [Route("edit/{slug}/{revisionDate?}", Name = RouteNames.WikiArticleEdit)]
        public ActionResult Edit(string slug, ArticleRevisionDate revisionDate = null)
        {
            this.EnsureCanEditArticle(slug, this.User);

            var model = new WikiArticleEditViewModel(this.CurrentRepository, slug, revisionDate);
            return this.View(model);
        }

        [BodyClass("wiki info")]
        [EnsureSlug(RouteName = RouteNames.WikiArticleInfo)]
        [Route("info/{slug}", Name = RouteNames.WikiArticleInfo)]
        public ActionResult Info(string slug)
        {
            this.EnsureCanReadArticle(slug, this.User);

            try
            {
                var model = new WikiArticleInfoViewModel(this.CurrentRepository, slug);
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

        private string GetArticleCanonicalUrl(ArticleResult articleResult)
        {
            string canonicalUrl = null;

            if (articleResult.HasRedirect)
            {
                canonicalUrl = this.Url.WikiArticle(articleResult.ArticleRedirect.RedirectToArticleSlug);
            }
            else if (articleResult.HasArticle)
            {
                canonicalUrl = this.Url.WikiArticle(articleResult.Article.Id);
            }

            return canonicalUrl;
        }
    }
}