using System.Linq;

using Raven.Client.Indexes;
using WikiDown.Security;

namespace WikiDown.RavenDb.Indexes
{
    public class ArticleAccessIndex : AbstractIndexCreationTask<ArticleAccess>
    {
        public ArticleAccessIndex()
        {
            this.Map =
                accesses => from access in accesses select new { access.CanRead, access.CanEdit, access.CanAdmin };
        }
    }
}