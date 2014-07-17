using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

using AspNetSeo;

namespace WikiDown.Website.ViewModels
{
    public class WikiSearchViewModel : ISeoModel
    {
        public WikiSearchViewModel(RequestContext requestContext, string searchTerm)
        {
            this.SearchTerm = searchTerm ?? string.Empty;

            var searchTermArticleId = new ArticleId(this.SearchTerm);

            this.SearchTermFormatted = Capitalize(searchTermArticleId.Title);

            var repository = RepositoryRequestInstance.Get(requestContext);

            var searchResult = repository.SearchArticles(this.SearchTerm);

            this.SearchResults = searchResult.Items.Select(x => new SearchItem(x)).ToList();
            this.Suggestions = searchResult.Suggestions;

            var articleExists = repository.GetArticleExists(searchTermArticleId);

            this.ArticleExists = (articleExists != null);

            bool isRedirect = (articleExists != null) && (articleExists.Title != SearchTermFormatted);

            this.ArticleTitle = (articleExists != null)
                                    ? (isRedirect ? articleExists.Title : this.SearchTermFormatted)
                                    : this.SearchTermFormatted;

            this.ArticleRedirectedFromTitle = (articleExists != null && isRedirect) ? this.SearchTermFormatted : null;
        }

        public bool ArticleExists { get; set; }

        public string ArticleTitle { get; set; }

        public string ArticleRedirectedFromTitle { get; set; }

        public string PageTitle
        {
            get
            {
                return string.Format("Search: {0}", this.SearchTerm);
            }
        }

        public IReadOnlyCollection<SearchItem> SearchResults { get; private set; }

        public string SearchTerm { get; private set; }

        public string SearchTermFormatted { get; private set; }

        public IReadOnlyCollection<string> Suggestions { get; private set; }

        private static string Capitalize(string str)
        {
            return !string.IsNullOrWhiteSpace(str) ? char.ToUpper(str[0]) + str.Substring(1) : string.Empty;
        }

        public class SearchItem
        {
            public SearchItem(ArticleSearchResultItem item)
            {
                this.Slug = item.Slug;
                this.Highlighting = new HtmlString(item.Highlighting);
            }

            public IHtmlString Highlighting { get; set; }

            public string Slug { get; set; }

            public string Title
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(this.Slug) ? ArticleSlugUtility.Decode(this.Slug) : null;
                }
            }
        }

        public void PopulateSeo(SeoHelper seoHelper)
        {
            seoHelper.Title = string.Format("Search: {0}", this.SearchTermFormatted);
        }
    }
}