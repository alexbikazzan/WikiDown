using System;
using System.Web.Mvc;

using AspNetSeo;
using WikiDown.Website.ViewModels;

namespace WikiDown.Website.Controllers
{
    public class WikiController : ControllerBase
    {
        public WikiController()
            : this(null)
        {
        }

        public WikiController(Repository repository)
            : base(repository)
        {
        }

        [Route("wiki", Name = RouteNames.WikiList)]
        [SeoTitle("List")]
        [BodyClass("wiki list")]
        public ActionResult List()
        {
            var model = new WikiListViewModel(this.CurrentRepository);
            return this.View(model);
        }

        [Route("wiki/{slug}/{revisionDate?}", Name = RouteNames.WikiArticle)]
        [BodyClass("wiki article")]
        public ActionResult Article(string slug, ArticleRevisionDate revisionDate = null, bool redirect = true)
        {
            try
            {
                var articleResult = this.CurrentRepository.GetArticleResult(slug, revisionDate);

                this.Seo.CanonicalUrl = this.GetArticleCanonicalUrl(articleResult);

                var model = new WikiArticleViewModel(slug, articleResult, revisionDate, redirect);
                return this.View(model);
            }
            catch (ArticleIdNotEnsuredException ex)
            {
                string ensuredArticleRedirectUrl = this.Url.WikiArticle(ex.EnsuredSlug);
                return this.RedirectPermanent(ensuredArticleRedirectUrl);
            }
        }

        [Route("delete/{slug}/{revisionDate}", Name = RouteNames.WikiArticleRevisionDelete)]
        public ActionResult DeleteRevision(string slug, ArticleRevisionDate revisionDate)
        {
            if (revisionDate == null || !revisionDate.HasValue)
            {
                throw new ArgumentNullException("revisionDate");
            }

            bool isDeleteSuccessful = this.CurrentRepository.DeleteArticleRevision(slug, revisionDate);
            if (!isDeleteSuccessful)
            {
                string message = string.Format(
                    "No article-revision found for slug '{0}' and revision-date '{1}'.",
                    slug,
                    revisionDate);
                throw new ArgumentOutOfRangeException("revisionDate", message);
            }

            string redirectUrl = this.Url.WikiArticleInfo(slug);
            return this.Redirect(redirectUrl);
        }

        [HttpGet]
        [Route("edit/{slug}/{revisionDate?}", Name = RouteNames.WikiArticleEdit)]
        [SeoMetaNoIndex]
        [BodyClass("wiki edit")]
        public ActionResult Edit(string slug, ArticleRevisionDate revisionDate = null)
        {
            var model = new WikiArticleEditViewModel(this.CurrentRepository, slug, revisionDate);
            return this.View(model);
        }

        [HttpPost]
        [Route("edit/{slug}")]
        [ValidateInput(false)]
        [SeoMetaNoIndex]
        [BodyClass("wiki edit")]
        public ActionResult Edit(string slug, WikiArticleEditViewModel editedArticle)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(editedArticle);
            }

            var articleId = new ArticleId(slug);
            var savedArticle = editedArticle.Save(this.CurrentRepository, articleId);

            var savedArticleId = (savedArticle.HasArticle) ? savedArticle.Article.Id : articleId;
            return this.Redirect(url => url.WikiArticle(savedArticleId));
        }

        [Route("info/{slug}", Name = RouteNames.WikiArticleInfo)]
        [BodyClass("wiki info")]
        public ActionResult Info(string slug)
        {
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