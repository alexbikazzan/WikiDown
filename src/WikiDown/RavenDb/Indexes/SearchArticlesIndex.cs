using System.Linq;

using Raven.Client.Indexes;

namespace WikiDown.RavenDb.Indexes
{
    public class SearchArticlesIndex :AbstractMultiMapIndexCreationTask<SearchArticlesIndex.Result>
    {
        public SearchArticlesIndex()
        {
            this.AddMap<Article>(
                articles =>
                from article in articles
                //where !article.IsDeleted
                select new Result { Content = new object[] { article.Title, article.ActiveRevisionId } });

            this.Index(x => x.Content, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
        }

        public class Result
        {
            public object[] Content { get; set; }
        }
    }
}