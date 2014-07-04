using System.Diagnostics;

namespace WikiDown
{
    [DebuggerDisplay("Id={Id}, Original={OriginalArticleSlug}, RedirectTo={RedirectToArticleSlug}")]
    public class ArticleRedirect
    {
        public ArticleRedirect()
        {
        }

        public ArticleRedirect(ArticleId originalArticleId, ArticleId redirectToArticleSlug)
        {
            this.OriginalArticleSlug = originalArticleId.Slug;
            this.RedirectToArticleSlug = redirectToArticleSlug.Slug;
        }

        public string OriginalArticleSlug { get; set; }

        public string Id { get; set; }

        public string RedirectToArticleSlug { get; set; }
    }
}