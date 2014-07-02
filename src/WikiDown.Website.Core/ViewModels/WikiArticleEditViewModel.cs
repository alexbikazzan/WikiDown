using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleEditViewModel : WikiArticleViewModelBase
    {
        private const char RedirectTitlesSeparator = ';';

        public WikiArticleEditViewModel()
        {
        }

        public WikiArticleEditViewModel(Repository repository, ArticleId articleId, DateTime? revisionDateTime)
            : base(articleId)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            var article = repository.GetArticle(articleId);

            string articleRevisionId = revisionDateTime.HasValue
                                           ? ArticleId.CreateArticleRevisionId(articleId, revisionDateTime.Value)
                                           : ((article != null) ? article.ActiveRevisionId : null);

            var articleRevision = repository.GetArticleRevision(articleRevisionId);

            this.RevisionDateTime = revisionDateTime
                                    ?? ((articleRevision != null) ? articleRevision.CreatedAt : (DateTime?)null);

            this.MarkdownContent = (articleRevision != null) ? articleRevision.MarkdownContent : null;

            this.IsCreateMode = (article == null || article.CreatedAt == DateTime.MinValue);

            this.MetaKeywords = (article != null) ? article.MetaKeywords : null;

            var redirectTitles = (article != null) ? article.RedirectTitles : Enumerable.Empty<string>();
            this.RedirectTitles = string.Join(RedirectTitlesSeparator + " ", redirectTitles);
        }

        public bool IsCreateMode { get; private set; }

        [AllowHtml]
        public string MarkdownContent { get; set; }

        public string MetaKeywords { get; set; }

        public override string PageTitle
        {
            get
            {
                string modeText = this.IsCreateMode ? "Create" : "Edit";
                return string.Format("{0}: {1}", modeText, this.ArticleTitle);
            }
        }

        public string RedirectTitles { get; set; }

        public DateTime? RevisionDateTime { get; set; }

        public string RevisionEditSummary { get; set; }

        public Article Save(Repository repository, ArticleId articleId)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            var redirectTitles = GetRedirectTitles();

            var article = repository.GetArticle(articleId) ?? new Article(articleId, this.MetaKeywords, redirectTitles);

            var articleRevision = new ArticleRevision(this.MarkdownContent, this.RevisionEditSummary);

            return repository.SaveArticle(article, articleRevision);
        }

        private IEnumerable<string> GetRedirectTitles()
        {
            var redirectTitles = this.RedirectTitles ?? string.Empty;

            return
                redirectTitles.Split(RedirectTitlesSeparator)
                    .Where(
                        x =>
                        !string.IsNullOrWhiteSpace(x)
                        && !x.Equals(this.ArticleTitle, StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => x.Trim())
                    .Distinct()
                    .ToList();
        }
    }
}