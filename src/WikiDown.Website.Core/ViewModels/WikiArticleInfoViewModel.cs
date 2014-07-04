using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleInfoViewModel : WikiArticleViewModelBase
    {
        public WikiArticleInfoViewModel(Repository repository, ArticleId articleId)
            : base(articleId, activeTab: ArticleHeaderTab.Info)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            var article = repository.GetArticle(articleId);
            if (article == null)
            {
                string message = string.Format("No article found for '{0}'.", articleId.Slug);
                throw new ArgumentOutOfRangeException("articleId", message);
            }

            this.ActiveArticleRevisionId = article.ActiveRevisionId;

            this.ArticleRedirects =
                repository.GetArticleRedirects(articleId)
                    .Select(x => new KeyValuePair<string, string>(x.Slug, x.Title))
                    .ToList();

            var metaKeywords = article.MetaKeywords ?? string.Empty;
            this.MetaKeywords =
                metaKeywords.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();

            this.Revisions = (from revision in repository.GetArticleRevisions(articleId)
                              let revisionId = ArticleId.CreateArticleRevisionId(articleId, revision)
                              select new KeyValuePair<string, ArticleRevisionDate>(revisionId, revision)).ToList();
        }

        public string ActiveArticleRevisionId { get; set; }

        public IReadOnlyCollection<KeyValuePair<string, string>> ArticleRedirects { get; set; }

        public IReadOnlyCollection<string> MetaKeywords { get; set; }

        public IReadOnlyCollection<KeyValuePair<string, ArticleRevisionDate>> Revisions { get; set; }
    }
}