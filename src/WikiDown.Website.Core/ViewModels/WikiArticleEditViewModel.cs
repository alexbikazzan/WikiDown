using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleEditViewModel : WikiArticleViewModelBase
    {
        public WikiArticleEditViewModel(
            Repository repository,
            ArticleId articleId,
            ArticleRevisionDate articleRevisionDate = null)
            : base(articleId, articleRevisionDate, HeaderTab.Edit)
        {
            var article = repository.GetArticle(articleId);
            this.IsCreateMode = (article == null || article.CreatedAt == DateTime.MinValue);
        }

        //private const char RedirectTitlesSeparator = ';';

        //public WikiArticleEditViewModel(Repository repository, ArticleId articleId, DateTime? revisionDateTime)
        //    : base(articleId, activeTab: HeaderTab.Edit)
        //{
        //    if (repository == null)
        //    {
        //        throw new ArgumentNullException("repository");
        //    }

        //    var article = repository.GetArticle(articleId);

        //    string articleRevisionId = revisionDateTime.HasValue
        //                                   ? IdUtility.CreateArticleRevisionId(articleId, revisionDateTime.Value)
        //                                   : ((article != null) ? article.ActiveRevisionId : null);

        //    var articleRevision = repository.GetArticleRevision(articleRevisionId);

        //    this.RevisionDateTime = revisionDateTime
        //                            ?? ((articleRevision != null) ? articleRevision.CreatedAt : (DateTime?)null);

        //    this.MarkdownContent = (articleRevision != null) ? articleRevision.MarkdownContent : null;

        //    this.IsCreateMode = (article == null || article.CreatedAt == DateTime.MinValue);

        //    this.MetaKeywords = (article != null) ? article.MetaKeywords : null;

        //    var articleRedirects = repository.GetArticleRedirectList(articleId).Select(x => x.Title);
        //    this.ArticleRedirects = string.Join(RedirectTitlesSeparator + " ", articleRedirects);
        //}

        //public string ArticleRedirects { get; set; }

        public bool IsCreateMode { get; private set; }

        //[AllowHtml]
        //public string MarkdownContent { get; set; }

        //public string MetaKeywords { get; set; }

        public override string PageTitle
        {
            get
            {
                string modeText = this.IsCreateMode ? "Create" : "Edit";
                return string.Format("{0}: {1}", modeText, this.ArticleTitle);
            }
        }

        //public DateTime? RevisionDateTime { get; set; }

        //public string RevisionEditSummary { get; set; }

        //public ArticleResult Save(Repository repository, ArticleId articleId)
        //{
        //    if (articleId == null)
        //    {
        //        throw new ArgumentNullException("articleId");
        //    }

        //    var article = repository.GetArticle(articleId) ?? new Article(articleId, this.MetaKeywords);

        //    var articleRevision = new ArticleRevision(this.MarkdownContent, this.RevisionEditSummary);

        //    var articleRedirects = GetArticleRedirects(articleId).ToArray();

        //    return repository.SaveArticle(article, articleRevision, articleRedirects);
        //}

        //private IEnumerable<ArticleRedirect> GetArticleRedirects(ArticleId articleId)
        //{
        //    var articleRedirectItems = (this.ArticleRedirects ?? string.Empty).Split(RedirectTitlesSeparator);
        //    var articleRedirects =
        //        articleRedirectItems.Where(
        //            x =>
        //            !string.IsNullOrWhiteSpace(x)
        //            && !x.Equals(this.ArticleTitle, StringComparison.InvariantCultureIgnoreCase))
        //            .Select(x => x.Trim())
        //            .Distinct()
        //            .ToList();

        //    return articleRedirects.Select(x => new ArticleRedirect(x, articleId));
        //}
    }
}