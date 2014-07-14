using System;
using System.Security.Principal;

using Raven.Imports.Newtonsoft.Json;

namespace WikiDown
{
    public class ArticleRevision
    {
        public ArticleRevision(
            ArticleId articleId,
            IPrincipal principal,
            string markdownContent,
            string editSummary = null)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            this.ArticleId = articleId.Id;
            this.CreatedByUserName = principal.Identity.Name;
            this.EditSummary = editSummary;
            this.MarkdownContent = markdownContent;

            this.CreatedAt = DateTime.UtcNow;
        }

        [JsonConstructor]
        private ArticleRevision()
        {
        }

        public string ArticleId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string CreatedByUserName { get; set; }

        public string EditSummary { get; set; }

        public string Id { get; set; }

        public DateTime? LastPublishedAt { get; set; }

        public string MarkdownContent { get; set; }
    }
}