using System;
using System.Security.Principal;
using System.Web.Routing;

using AspNetSeo;
using WikiDown.Security;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleViewModelBase : ISeoModel
    {
        protected readonly Repository CurrentRepository;

        protected readonly IPrincipal CurrentPrincipal;

        public WikiArticleViewModelBase(
            RequestContext requestContext,
            ArticleId articleId,
            ArticleRevisionDate articleRevisionDate = null,
            HeaderTab activeTab = HeaderTab.None)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            this.CurrentRepository = RepositoryRequestInstance.Get(requestContext);
            this.CurrentPrincipal = requestContext.HttpContext.User;

            this.ArticleId = articleId;
            this.DisplayArticleId = articleId;
            this.ArticleRevisionDate = articleRevisionDate;
            this.ActiveTab = activeTab;

            this.Article = this.CurrentRepository.GetArticle(articleId);

            this.CanEditArticle = (this.Article == null) || this.Article.CanEdit(this.CurrentPrincipal);
        }

        protected Article Article { get; private set; }

        public HeaderTab ActiveTab { get; private set; }

        public DateTime? ArticleRevisionDate { get; private set; }

        public ArticleId ArticleId { get; protected set; }

        public string ArticleSlug
        {
            get
            {
                return this.ArticleId.HasValue ? this.ArticleId.Slug : null;
            }
        }

        public string ArticleTitle
        {
            get
            {
                return this.DisplayArticleId.HasValue ? this.DisplayArticleId.Title : this.ArticleId.Title;
            }
        }

        public bool CanEditArticle { get; set; }

        public ArticleId DisplayArticleId { get; set; }

        public virtual string PageTitle
        {
            get
            {
                return this.ArticleTitle;
            }
        }

        public virtual void PopulateSeo(SeoHelper seoHelper)
        {
            seoHelper.Title = this.PageTitle;

            var article = this.CurrentRepository.GetArticle(this.ArticleId);
            if (article != null && article.ArticleAccess.CanRead > ArticleAccessLevel.Anonymous)
            {
                seoHelper.MetaNoIndex = true;
            }
        }

        public enum HeaderTab
        {
            None,

            Article,

            Edit,

            Info
        }
    }
}