using System;

using WikiDown.Security;

namespace WikiDown
{
    public class ArticleRevisionItem
    {
        public string ArticleId { get; set; }

        public string ArticleTitle { get; set; }

        public ArticleAccessLevel CanReadRole { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedAtFormatted
        {
            get
            {
                return this.CreatedAt.ToString(ArticleRevisionDate.FormattedDateTimeFormat);
            }
        }

        public string CreatedByUserName { get; set; }

        public string EditSummary { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastPublishedAt { get; set; }

        public string LastPublishedAtId
        {
            get
            {
                return this.LastPublishedAt.HasValue
                           ? this.LastPublishedAt.Value.ToString(ArticleRevisionDate.IdDateTimeFormat)
                           : null;
            }
        }

        public string LastPublishedAtFormatted
        {
            get
            {
                return this.LastPublishedAt.HasValue
                           ? this.LastPublishedAt.Value.ToString(ArticleRevisionDate.FormattedDateTimeFormat)
                           : null;
            }
        }
    }
}