using System;

namespace WikiDown
{
    public class ArticleRevision
    {
        public const string IdDateTimeFormat = "yyyyMMdd-HHmmss-fffffff";

        public const string ReadableDateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public ArticleRevision()
        {
        }

        public ArticleRevision(string markdownContent, string editSummary = null)
        {
            this.MarkdownContent = markdownContent;
            this.EditSummary = editSummary;
            this.CreatedAt = DateTime.UtcNow;
        }

        public string ArticleId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string EditSummary { get; set; }

        public string Id { get; set; }

        public string MarkdownContent { get; set; }
    }
}