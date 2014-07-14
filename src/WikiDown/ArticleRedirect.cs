using System.Diagnostics;

using Raven.Imports.Newtonsoft.Json;

namespace WikiDown
{
    [DebuggerDisplay("Id={Id}, Original={OriginalArticleSlug}, RedirectTo={RedirectToArticleSlug}")]
    public class ArticleRedirect
    {

        public ArticleRedirect(ArticleId originalArticleId, ArticleId redirectToArticleSlug)
        {
            this.OriginalArticleSlug = originalArticleId.Slug;
            this.RedirectToArticleSlug = redirectToArticleSlug.Slug;
        }

        [JsonConstructor]
        private ArticleRedirect()
        {
        }

        public string OriginalArticleSlug { get; set; }

        public string Id { get; set; }

        public string RedirectToArticleSlug { get; set; }
    }
}