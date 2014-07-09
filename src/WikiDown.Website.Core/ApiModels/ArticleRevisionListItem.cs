namespace WikiDown.Website.ApiModels
{
    public class ArticleRevisionListItem : IdTextApiModel<string>
    {
        public ArticleRevisionListItem(ArticleRevisionDate articleRevisionDate, bool isActive)
        {
            this.Id = articleRevisionDate.DateTime.ToString(ArticleRevision.IdDateTimeFormat);
            this.Text = articleRevisionDate.DateTime.ToString(ArticleRevision.ReadableDateTimeFormat);
            this.IsActive = isActive;
        }

        public bool IsActive { get; set; }
    }
}