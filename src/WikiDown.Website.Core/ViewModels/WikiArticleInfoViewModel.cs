using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleInfoViewModel : WikiArticleViewModelBase
    {
        public WikiArticleInfoViewModel(Repository repository, ArticleId articleId)
            : base(articleId)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            var article = repository.GetArticle(articleId);

            this.RedirectTitles =
                ((article != null) ? article.RedirectTitles : Enumerable.Empty<string>()).Select(x => new ArticleId(x))
                    .ToList();

            this.MetaKeywords = (article != null) ? article.MetaKeywords : null;

            this.Revisions = repository.GetArticleRevisions(articleId);
        }

        public string MetaKeywords { get; set; }

        public IReadOnlyCollection<ArticleId> RedirectTitles { get; set; }

        public IReadOnlyCollection<ArticleRevisionDate> Revisions { get; set; }

        public override string PageTitle
        {
            get
            {
                return string.Format("Info: {0}", this.ArticleTitle);
            }
        }
    }
}