using System;

using AspNetSeo;

namespace WikiDown.Website.ViewModels
{
    public abstract class WikiArticleViewModelBase : ISeoModel
    {
        internal WikiArticleViewModelBase()
        {
        }

        protected WikiArticleViewModelBase(ArticleId articleId)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            this.ArticleSlug = articleId.Slug;
            this.ArticleTitle = articleId.Title;
        }

        public string ArticleSlug { get; set; }

        public string ArticleTitle { get; set; }

        public virtual string PageTitle
        {
            get
            {
                return this.ArticleTitle;
            }
        }

        public void Populate(SeoHelper seoHelper)
        {
            seoHelper.Title = this.PageTitle;
        }
    }
}