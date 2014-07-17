using System.Diagnostics;

using Raven.Imports.Newtonsoft.Json;

namespace WikiDown
{
    [DebuggerDisplay("Id={Id}, RedirectFrom={RedirectFromArticleSlug}, RedirectTo={RedirectToArticleSlug}")]
    public class ArticleRedirect
    {

        public ArticleRedirect(ArticleId redirectFromArticleId, ArticleId redirectToArticleId)
        {
            this.RedirectFromArticleSlug = redirectFromArticleId.Slug;
            this.RedirectToArticleSlug = redirectToArticleId.Slug;
        }

        [JsonConstructor]
        private ArticleRedirect()
        {
        }

        public string Id { get; set; }

        public string RedirectFromArticleSlug { get; set; }

        public string RedirectToArticleSlug { get; set; }
    }
}