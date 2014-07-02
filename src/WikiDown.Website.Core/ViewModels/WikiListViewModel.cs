using System;
using System.Collections.Generic;

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

            this.Articles = repository.GetArticles();
        }

        public IReadOnlyCollection<KeyValuePair<string, string>> Articles { get; set; }
    }
}