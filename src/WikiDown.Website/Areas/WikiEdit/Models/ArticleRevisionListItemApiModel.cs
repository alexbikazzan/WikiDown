using WikiDown.Website.ApiModels;

namespace WikiDown.Website.Areas.WikiEdit.Models
{
    public class ArticleRevisionListItemApiModel : IdTextApiModel<string>
    {
        public ArticleRevisionListItemApiModel(ArticleRevisionItem articleRevision)
        {
            this.CreatedByUserName = articleRevision.CreatedByUserName;
            this.EditSummary = articleRevision.EditSummary;
            this.IsActive = articleRevision.IsActive;

            var articleRevisionDate = new ArticleRevisionDate(articleRevision.CreatedAt);

            this.Id = articleRevisionDate.DateTimeId;
            this.Text = articleRevisionDate.DateTimeFormatted;

            var lastPublished = articleRevision.LastPublishedAt;
            this.LastPublishedAt = lastPublished.HasValue
                                       ? lastPublished.Value.ToString(ArticleRevisionDate.FormattedDateTimeFormat)
                                       : null;
        }

        public string CreatedByUserName { get; set; }

        public string EditSummary { get; set; }

        public bool IsActive { get; set; }

        public string LastPublishedAt { get; set; }
    }
}