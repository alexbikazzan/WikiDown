using System.Linq;

using Raven.Client.Indexes;

namespace WikiDown.RavenDb.Indexes
{
    public class ArticleRedirectsIndex : AbstractIndexCreationTask<ArticleRedirect>
    {
        public ArticleRedirectsIndex()
        {
            this.Map =
                redirects =>
                from redirect in redirects select new { redirect.OriginalArticleSlug, redirect.RedirectToArticleSlug };
        }
    }
}