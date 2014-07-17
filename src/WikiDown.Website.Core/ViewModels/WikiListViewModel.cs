using System;
using System.Collections.Generic;
using System.Linq;

namespace WikiDown.Website.ViewModels
{
    public class WikiListViewModel
    {
        public WikiListViewModel(Repository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            
            var drafts = repository.GetArticleDraftsList();
            this.Drafts = drafts.Select(x => new KeyValuePair<string, string>(x.Slug, x.Title)).ToList();

            var changedArticles = repository.GetArticleRevisionsLatestChangesList();
            this.LatestChangedArticles = changedArticles.ToList();
        }
        
        public IReadOnlyCollection<KeyValuePair<string, string>> Drafts { get; set; }

        public IReadOnlyCollection<ArticleRevisionItem> LatestChangedArticles { get; set; }
    }
}