using System.Linq;

namespace WikiDown.RavenDb.Indexes
{
    public class ArticleRevisionsChangedIndex : ArticleRevisionsIndex
    {
        public ArticleRevisionsChangedIndex()
        {
            this.Reduce = revisions => from revision in revisions
                                       group revision by revision.ArticleId
                                       into g
                                       let rev = g.OrderBy(x => x.CreatedAt).Last()
                                       select
                                           new
                                               {
                                                   rev.ActiveRevisionId,
                                                   rev.ArticleId,
                                                   rev.ArticleSlug,
                                                   rev.CanReadRole,
                                                   rev.CreatedAt,
                                                   rev.CreatedByUserName,
                                                   rev.IsActive,
                                                   rev.LastPublishedAt
                                               };
        }
    }
}