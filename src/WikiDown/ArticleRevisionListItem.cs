using System;

namespace WikiDown
{
    public class ArticleRevisionListItem
    {
        public string ArticleId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedByUserName { get; set; }

        public string EditSummary { get; set; }

        public bool IsActive { get; set; }

        public DateTime? LastPublishedAt { get; set; }
    }
}