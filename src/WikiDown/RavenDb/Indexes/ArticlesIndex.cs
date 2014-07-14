using System.Linq;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace WikiDown.RavenDb.Indexes
{
    public class ArticlesIndex : AbstractIndexCreationTask<Article, ArticlesIndex.Result>
    {
        public ArticlesIndex()
        {
            this.Map = articles => from article in articles
                                   select
                                       new
                                           {
                                               ArticleId = article.Id,
                                               ArticleTitle = article.Title,
                                               ActiveRevisionId = article.ActiveRevisionId,
                                               CanReadAccess = article.ArticleAccess.CanRead
                                           };

            this.Index(x => x.CanReadAccess, FieldIndexing.Analyzed);
        }

        public class Result
        {
            public string ActiveRevisionId { get; set; }

            public string ArticleId { get; set; }

            public string ArticleTitle { get; set; }

            public int CanReadAccess { get; set; }
        }
    }
}