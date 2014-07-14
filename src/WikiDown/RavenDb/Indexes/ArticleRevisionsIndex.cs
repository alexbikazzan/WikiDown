using System;
using System.Linq;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace WikiDown.RavenDb.Indexes
{
    public class ArticleRevisionsIndex : AbstractIndexCreationTask<ArticleRevision, ArticleRevisionsIndex.Result>
    {
        public ArticleRevisionsIndex()
        {
            this.Map = revisions => from revision in revisions
                                    let article = this.LoadDocument<Article>(revision.ArticleId)
                                    where article != null
                                    let activeRevisionId = article.ActiveRevisionId
                                    select
                                        new
                                            {
                                                ArticleId = revision.ArticleId,
                                                ArticleTitle = article.Title,
                                                ActiveRevisionId = article.ActiveRevisionId,
                                                CanReadRole = (int)article.ArticleAccess.CanRead,
                                                CreatedAt = revision.CreatedAt,
                                                CreatedByUserName = revision.CreatedByUserName,
                                                LastPublishedAt = revision.LastPublishedAt,
                                                IsActive = activeRevisionId == revision.Id
                                            };

            this.Index(x => x.ArticleTitle, FieldIndexing.Analyzed);
            this.Index(x => x.CanReadRole, FieldIndexing.Analyzed);
            this.Index(x => x.IsActive, FieldIndexing.Analyzed);

            this.Store(x => x.IsActive, FieldStorage.Yes);
        }

        public class Result
        {
            public string ActiveRevisionId { get; set; }

            public string ArticleId { get; set; }

            public string ArticleTitle { get; set; }

            public int CanReadRole { get; set; }

            public DateTime CreatedAt { get; set; }

            public string CreatedByUserName { get; set; }

            public DateTime? LastPublishedAt { get; set; }

            public bool IsActive { get; set; }
        }
    }
}