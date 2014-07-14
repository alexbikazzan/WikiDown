using Raven.Imports.Newtonsoft.Json;

namespace WikiDown.Security
{
    public class ArticleAccess
    {
        public ArticleAccess(string articleId)
        {
            this.ArticleId = articleId;
        }

        [JsonConstructor]
        private ArticleAccess()
        {
        }

        public string ArticleId { get; set; }

        public ArticleAccessLevel CanRead { get; set; }

        public ArticleAccessLevel CanEdit { get; set; }

        public ArticleAccessLevel CanAdmin { get; set; }

        public string Id { get; set; }

        public static ArticleAccess Default(ArticleId articleId)
        {
            return new ArticleAccess(articleId.Id)
                       {
                           CanAdmin = ArticleAccessLevel.Admin,
                           CanEdit = ArticleAccessLevel.Editor,
                           CanRead = ArticleAccessLevel.Anonymous
                       };
        }
    }
}