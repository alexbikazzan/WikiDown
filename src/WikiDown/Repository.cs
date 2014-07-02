using System;
using System.Collections.Generic;
using System.Linq;

using Raven.Client;
using Raven.Client.Linq;

namespace WikiDown
{
    public class Repository : IDisposable
    {
        private readonly Lazy<IAsyncDocumentSession> currentAsyncSessionLazy;

        private readonly Lazy<IDocumentSession> currentSessionLazy;

        private readonly IDocumentStore documentStore;

        public Repository(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;

            this.currentAsyncSessionLazy = new Lazy<IAsyncDocumentSession>(() => this.documentStore.OpenAsyncSession());
            this.currentSessionLazy = new Lazy<IDocumentSession>(() => this.documentStore.OpenSession());
        }

        private IAsyncDocumentSession CurrentAsyncSession
        {
            get
            {
                return this.currentAsyncSessionLazy.Value;
            }
        }

        private IDocumentSession CurrentSession
        {
            get
            {
                return this.currentSessionLazy.Value;
            }
        }

        public virtual Article GetArticle(ArticleId articleId)
        {
            return (articleId != null && articleId.HasValue) ? this.CurrentSession.Load<Article>(articleId.Id) : null;
        }

        public IReadOnlyCollection<KeyValuePair<string, string>> GetArticles()
        {
            using (var session = this.GetMaxRequestsSession())
            {
                var result = (from article in session.Query<Article>()
                              where article.ActiveRevisionId != null
                              orderby article.Title
                              select new { article.Id, article.Title }).ToList();

                return result.Select(x => new KeyValuePair<string, string>(x.Title, x.Id)).ToList();
            }
        }

        public IReadOnlyCollection<ArticleRevisionDate> GetArticleRevisions(ArticleId articleId)
        {
            if (articleId == null || !articleId.HasValue)
            {
                return Enumerable.Empty<ArticleRevisionDate>().ToList();
            }

            using (var session = this.GetMaxRequestsSession())
            {
                var result = (from revision in session.Query<ArticleRevision>()
                              where revision.ArticleId == articleId.Id
                              orderby revision.CreatedAt descending
                              select new { revision.CreatedAt }).ToList();

                return result.Select(x => new ArticleRevisionDate(x.CreatedAt)).ToList();
            }
        }

        public ArticleRevision GetArticleRevision(ArticleId articleId, ArticleRevisionDate articleRevisionDate)
        {
            string id = (articleId != null) ? articleId.Id : null;

            string articleRevisionId = ArticleId.CreateArticleRevisionId(id, articleRevisionDate);

            return this.GetArticleRevision(articleRevisionId);
        }

        public virtual ArticleRevision GetArticleRevision(string articleRevisionId)
        {
            return (articleRevisionId != null) ? this.CurrentSession.Load<ArticleRevision>(articleRevisionId) : null;
        }

        public IDictionary<string, DateTime> GetArticleRevisionsHistory(ArticleId articleId)
        {
            if (articleId == null || !articleId.HasValue)
            {
                return new Dictionary<string, DateTime>(0);
            }

            using (var session = this.GetMaxRequestsSession())
            {
                var result = (from revision in session.Query<ArticleRevision>()
                              where revision.ArticleId == articleId.Id
                              orderby revision.CreatedAt descending
                              select new { revision.Id, revision.CreatedAt }).ToList();

                return result.ToDictionary(x => x.Id, x => x.CreatedAt);
            }
        }

        public ArticlePage GetArticlePage(string articleIdOrSlug, DateTime? revisionCreated = null)
        {
            if (articleIdOrSlug == null)
            {
                throw new ArgumentNullException("articleIdOrSlug");
            }

            var articleId = new ArticleId(articleIdOrSlug);

            var redirectEnsuredArticle = EnsureEncodedArticleSlug(articleIdOrSlug, articleId);
            if (redirectEnsuredArticle != null)
            {
                return redirectEnsuredArticle;
            }

            var article = this.GetArticle(articleId);
            if (article == null)
            {
                // TODO: Try get redirected title

                return ArticlePage.ForNotFound();
            }

            //if (!string.IsNullOrWhiteSpace(article.RedirectToArticleId))
            //{
            //    var redirectArticleId = new ArticleId(article.RedirectToArticleId);
            //    return ArticlePage.ForRedirect(redirectArticleId.Slug);
            //}

            string articleRevisionId = revisionCreated.HasValue
                                           ? ArticleId.CreateArticleRevisionId(article.Id, revisionCreated.Value)
                                           : article.ActiveRevisionId;
            var articleRevision = this.GetArticleRevision(articleRevisionId);

            return ArticlePage.ForArticle(articleId, article, articleRevision);
        }

        public Article SaveArticle(Article article, ArticleRevision articleRevision)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            if (string.IsNullOrWhiteSpace(article.Id))
            {
                this.CurrentSession.Store(article);
            }

            this.SaveArticleRevision(article, articleRevision);

            this.CurrentSession.SaveChanges();

            return article;
        }

        public void Dispose()
        {
            if (this.CurrentSession != null)
            {
                this.CurrentSession.Dispose();
            }

            if (this.CurrentAsyncSession != null)
            {
                this.CurrentAsyncSession.Dispose();
            }
        }

        private static ArticlePage EnsureEncodedArticleSlug(string articleIdOrSlug, ArticleId articleId)
        {
            string ensuredSlug = articleId.Slug;

            return (ensuredSlug != articleIdOrSlug) ? ArticlePage.ForRedirect(ensuredSlug) : null;
        }

        private IDocumentSession GetMaxRequestsSession()
        {
            var session = this.documentStore.OpenSession();
            session.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;

            return session;
        }

        private void SaveArticleRevision(Article article, ArticleRevision articleRevision)
        {
            if (articleRevision == null)
            {
                return;
            }

            var latestArticleRevision = this.GetArticleRevision(article.ActiveRevisionId);
            if (latestArticleRevision != null
                && latestArticleRevision.MarkdownContent == articleRevision.MarkdownContent)
            {
                return;
            }

            articleRevision.ArticleId = article.Id;

            this.CurrentSession.Store(articleRevision);

            article.ActiveRevisionId = articleRevision.Id;
        }

#if DEBUG
        public IReadOnlyCollection<Article> DebugAllArticles()
        {
            using (var session = this.GetMaxRequestsSession())
            {
                return session.Query<Article>().Take(int.MaxValue).ToList();
            }
        }

        public IReadOnlyCollection<ArticleRevision> DebugAllArticleRevisions()
        {
            using (var session = this.GetMaxRequestsSession())
            {
                return session.Query<ArticleRevision>().Take(int.MaxValue).ToList();
            }
        }
#endif
    }
}