namespace WikiDown.Website.ApiModels
{
    public class ArticleRevisionsParams
    {
        public string ArticleId { get; set; }

        public string ArticleRevisionDate { get; set; }

        public ArticleId ArticleIdValue
        {
            get
            {
                return new ArticleId(this.ArticleId ?? string.Empty);
            }
        }

        public ArticleRevisionDate ArticleRevisionDateValue
        {
            get
            {
                return new ArticleRevisionDate(this.ArticleRevisionDate);
            }
        }
    }
}