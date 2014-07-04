using System.Linq;

using Raven.Client.Indexes;

namespace WikiDown.RavenDb.Indexes
{
    public class ActiveArticlesSlugsIndex : AbstractIndexCreationTask<Article>
    {
        public ActiveArticlesSlugsIndex()
        {
            this.Map =
                articles =>
                from article in articles
                where !article.IsDeleted
                select new { article.ActiveRevisionId, article.Id, article.Title };
        }
    }
}