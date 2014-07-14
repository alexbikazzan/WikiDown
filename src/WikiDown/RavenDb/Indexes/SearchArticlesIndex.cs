using System.Linq;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace WikiDown.RavenDb.Indexes
{
    public class SearchArticlesIndex : AbstractMultiMapIndexCreationTask<SearchArticlesIndex.Result>
    {
        public SearchArticlesIndex()
        {
            this.AddMap<Article>(
                articles => from article in articles
                            where !string.IsNullOrEmpty(article.ActiveRevisionId)
                            select new Result { Content = new object[] { article.Title } });

            this.AddMap<Article>(
                articles => from article in articles
                            where !string.IsNullOrEmpty(article.ActiveRevisionId)
                            from tag in article.Tags
                            select new Result { Content = new object[] { tag } });

            this.AddMap<ArticleRevision>(
                revisions =>
                from revision in revisions select new Result { Content = new object[] { revision.MarkdownContent } });

            this.AddMap<ArticleRedirect>(
                redirects =>
                from redirect in redirects select new Result { Content = new object[] { redirect.OriginalArticleSlug } });

            this.Index(x => x.Content, FieldIndexing.Analyzed);
        }

        public class Result
        {
            public object[] Content { get; set; }
        }
    }
}