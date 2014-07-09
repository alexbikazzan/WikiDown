using System.Linq;

using Raven.Client.Indexes;

namespace WikiDown.RavenDb.Indexes
{
    public class ArticleRevisionsIndex : AbstractIndexCreationTask<ArticleRevision>
    {
        public ArticleRevisionsIndex()
        {
            this.Map = revisions => from revision in revisions select new { revision.ArticleId, revision.CreatedAt };
        }
    }
}