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

            var articles = repository.GetArticleList();
            this.Articles = articles.Select(x => new KeyValuePair<string, string>(x.Slug, x.Title)).ToList();
        }

        public IReadOnlyCollection<KeyValuePair<string, string>> Articles { get; set; }
    }
}