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
        public ActionResult List()
        {
            var model = new WikiListViewModel(this.CurrentRepository);
            return this.View(model);
        }

        [Route("wiki/{slug}/{revisionCreated?}", Name = RouteNames.WikiArticle)]
        public ActionResult ArticleShow(
            string slug,
            ArticleRevisionDate revisionCreated = null,
            string from = null,
            int? noRedirect = 0)
        {
            var articlePage = this.CurrentRepository.GetArticlePage(slug, revisionCreated);

            if (!string.IsNullOrWhiteSpace(articlePage.RedirectArticleSlug))
            {
                string redirectUrl = this.Url.WikiArticle(articlePage.RedirectArticleSlug, revisionCreated);
                return this.Redirect(redirectUrl);
            }

            var articleId = new ArticleId(slug);

            if (!articlePage.HasArticle)
            {
                var notFoundArticleModel = new WikiArticleNotFoundViewModel(articleId);
                return this.View(ViewNames.WikiArticleNotFound, notFoundArticleModel);
            }

            var model = new WikiArticleViewModel(articlePage, articleId, revisionCreated, from);
            return this.View(ViewNames.WikiArticle, model);
        }

        [HttpGet]
        [Route("edit/{slug}/{revisionCreated?}", Name = RouteNames.WikiArticleEdit)]
        public ActionResult Edit(string slug, ArticleRevisionDate revisionCreated = null)
        {
            var model = new WikiArticleEditViewModel(this.CurrentRepository, slug, revisionCreated);
            return this.View(model);
        }

        [HttpPost]
        [Route("edit/{slug}")]
        [ValidateInput(false)]
        public ActionResult Edit(string slug, WikiArticleEditViewModel editedArticle)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(editedArticle);
            }

            var articleId = new ArticleId(slug);
            var savedArticle = editedArticle.Save(this.CurrentRepository, articleId);

            var savedArticleId = new ArticleId(savedArticle.Title);
            return this.Redirect(url => url.WikiArticle(savedArticleId));
        }

        [Route("info/{slug}", Name = RouteNames.WikiArticleInfo)]
        public ActionResult Info(string slug)
        {
            var model = new WikiArticleInfoViewModel(this.CurrentRepository, slug);
            return this.View(model);
        }

        [HttpPost]
        [Route("search", Name = RouteNames.WikiSearch)]
        public ActionResult Search(string search)
        {
            return this.Redirect(url => url.WikiArticle(search));
        }
    }
}