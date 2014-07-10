using System.Linq;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;
using WikiDown.Security;

namespace WikiDown.RavenDb.Indexes
{
    public class ArticlesListIndex : AbstractIndexCreationTask<Article, ArticlesListIndex.Result>
    {
        public ArticlesListIndex()
        {
            this.Map = articles => from article in articles
                                   let access = this.LoadDocument<ArticleAccess>(article.Id + "/ArticleAccess")
                                   let canRead = (access != null) ? access.CanRead : ArticleAccessRole.Anonymous
                                   select
                                       new
                                           {
                                               article.Title,
                                               article.CreatedByUserId,
                                               article.ActiveRevisionId,
                                               CanReadRole = canRead
                                           };

            this.Store(x => x.Title, FieldStorage.Yes);
            this.Store(x => x.ActiveRevisionId, FieldStorage.Yes);
            this.Store(x => x.CanReadRole, FieldStorage.Yes);
            this.Store(x => x.CreatedByUserId, FieldStorage.Yes);

            this.Sort(x => x.Title, SortOptions.String);
        }

        public class Result
        {
            public string ActiveRevisionId { get; set; }

            public string CreatedByUserId { get; set; }

            public string Title { get; set; }

            public int CanReadRole { get; set; }
        }
    }
}