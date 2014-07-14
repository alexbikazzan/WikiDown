using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

using Microsoft.AspNet.Identity;
using WikiDown.Website.Security;

namespace WikiDown.Website.ViewModels
{
    public class WikiArticleInfoViewModel : WikiArticleViewModelBase
    {
        private readonly UserManager<WikiDownUser> userManager;

        public WikiArticleInfoViewModel(
            RequestContext requestContext,
            ArticleId articleId,
            UserManager<WikiDownUser> userManager)
            : base(requestContext, articleId, activeTab: HeaderTab.Info)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }

            if (this.Article == null)
            {
                throw new ArticleNotFoundException("articleId");
            }

            this.userManager = userManager;

            this.ActiveArticleRevisionId = this.Article.ActiveRevisionId;

            var redirects = CurrentRepository.GetArticleRedirectsList(articleId);
            this.Redirects = redirects.Select(x => new KeyValuePair<string, string>(x.Slug, x.Title)).ToList();

            this.Tags = (this.Article.Tags ?? Enumerable.Empty<string>()).ToList();

            var articleRevisions = CurrentRepository.GetArticleRevisionsList(articleId);
            this.Revisions = articleRevisions.Select(x => new ArticleRevisionListItem(x, this.userManager)).ToList();
        }

        public string ActiveArticleRevisionId { get; set; }

        public IReadOnlyCollection<KeyValuePair<string, string>> Redirects { get; set; }

        public IReadOnlyCollection<ArticleRevisionListItem> Revisions { get; set; }

        public IReadOnlyCollection<string> Tags { get; set; }

        public class ArticleRevisionListItem
        {
            public ArticleRevisionListItem(ArticleRevisionItem articleRevision, UserManager<WikiDownUser> userManager)
            {
                var articleRevisionDate = new ArticleRevisionDate(articleRevision.CreatedAt);

                this.Date = articleRevisionDate;
                this.IsActive = articleRevision.IsActive;

                var createdByUser = !string.IsNullOrWhiteSpace(articleRevision.CreatedByUserName)
                                        ? userManager.FindByName(articleRevision.CreatedByUserName)
                                        : null;
                this.UserName = (createdByUser != null) ? createdByUser.UserName : null;
            }

            public ArticleRevisionDate Date { get; set; }

            public string DateFormatted
            {
                get
                {
                    return this.Date.DateTimeFormatted;
                }
            }

            public bool IsActive { get; set; }

            public string UserName { get; set; }
        }
    }
}