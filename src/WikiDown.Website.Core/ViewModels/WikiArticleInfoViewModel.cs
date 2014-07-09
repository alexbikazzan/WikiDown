using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleInfoViewModel : WikiArticleViewModelBase
    {
        public WikiArticleInfoViewModel(Repository repository, ArticleId articleId)
            : base(articleId, activeTab: HeaderTab.Info)
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
                repository.GetArticleRedirectList(articleId)
                    .Select(x => new KeyValuePair<string, string>(x.Slug, x.Title))
                    .ToList();

            this.Tags = (article.Tags ?? Enumerable.Empty<string>()).ToList();

            this.Revisions = (from revision in repository.GetArticleRevisionList(articleId)
                              let revisionId = IdUtility.CreateArticleRevisionId(articleId, revision)
                              select new KeyValuePair<string, ArticleRevisionDate>(revisionId, revision)).ToList();
        }

        public string ActiveArticleRevisionId { get; set; }

        public IReadOnlyCollection<KeyValuePair<string, string>> ArticleRedirects { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }

        public IReadOnlyCollection<KeyValuePair<string, ArticleRevisionDate>> Revisions { get; set; }
    }
}