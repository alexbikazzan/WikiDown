using System;

using AspNetSeo;

namespace WikiDown.Website.ViewModels
{
    public abstract class WikiArticleViewModelBase : ISeoModel
    {
        internal WikiArticleViewModelBase()
        {
        }

        protected WikiArticleViewModelBase(
            ArticleId articleId,
            ArticleRevisionDate articleRevisionDate = null,
            ArticleHeaderTab activeTab = ArticleHeaderTab.None)
        {
            this.ArticleId = articleId;
            this.DisplayArticleId = articleId;

            this.ArticleRevisionDate = articleRevisionDate;
            this.ActiveTab = activeTab;
        }

        public ArticleHeaderTab ActiveTab { get; set; }

        public DateTime? ArticleRevisionDate { get; set; }

        public ArticleId ArticleId { get; set; }

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
        }
    }
}