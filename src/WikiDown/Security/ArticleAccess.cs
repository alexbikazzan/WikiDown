namespace WikiDown.Security
{
    public class ArticleAccess
    {
        public ArticleAccessLevel CanRead { get; set; }

        public ArticleAccessLevel CanEdit { get; set; }

        public ArticleAccessLevel CanAdmin { get; set; }

        public static ArticleAccess Default()
        {
            return new ArticleAccess
                       {
                           CanAdmin = ArticleAccessLevel.Admin,
                           CanEdit = ArticleAccessLevel.Editor,
                           CanRead = ArticleAccessLevel.Anonymous
                       };
        }
    }
}