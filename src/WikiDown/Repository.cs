using System;
using System.Collections.Generic;
using System.Linq;

using Raven.Client;
using Raven.Client.Linq;
using WikiDown.RavenDb.Indexes;
using WikiDown.Security;

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

        internal IAsyncDocumentSession CurrentAsyncSession
        {
            get
            {
                return this.currentAsyncSessionLazy.Value;
            }
        }

        internal IDocumentSession CurrentSession
        {
            get
            {
                return this.currentSessionLazy.Value;
            }
        }

        public bool DeleteArticleRevision(ArticleId articleId, ArticleRevisionDate revisionDate)
        {
            var articleRevision = this.GetArticleRevision(articleId, revisionDate);
            if (articleRevision == null)
            {
                return false;
            }

            this.CurrentSession.Delete(articleRevision);

            this.CurrentSession.SaveChanges();
            return true;
        }

        public virtual Article GetArticle(ArticleId articleId)
        {
            return (articleId != null && articleId.HasValue) ? this.CurrentSession.Load<Article>(articleId.Id) : null;
        }

        public IReadOnlyCollection<ArticleId> GetArticleList()
        {
            var result = (from article in this.CurrentSession.Query<Article, ActiveArticlesSlugsIndex>()
                          where article.ActiveRevisionId != null
                          orderby article.Title
                          select new { article.Title }).ToList();

            return result.Select(x => new ArticleId(x.Title)).ToList();
        }

        public ArticleRedirect GetArticleRedirect(ArticleId originalArticleId)
        {
            string articleRedirectId = IdUtility.CreateArticleRedirectId(originalArticleId);

            return this.CurrentSession.Load<ArticleRedirect>(articleRedirectId);
        }

        public IReadOnlyCollection<ArticleId> GetArticleRedirectList(ArticleId redirectToArticleId)
        {
            if (redirectToArticleId == null || !redirectToArticleId.HasValue)
            {
                return Enumerable.Empty<ArticleId>().ToList();
            }

            var result = (from redirect in this.CurrentSession.Query<ArticleRedirect>()
                          where redirect.RedirectToArticleSlug == redirectToArticleId.Slug
                          select new { redirect.OriginalArticleSlug }).ToList();

            return result.Select(x => new ArticleId(x.OriginalArticleSlug)).ToList();
        }

        public IReadOnlyCollection<ArticleRevisionDate> GetArticleRevisionList(ArticleId articleId)
        {
            if (articleId == null || !articleId.HasValue)
            {
                return Enumerable.Empty<ArticleRevisionDate>().ToList();
            }

            var result = (from revision in this.CurrentSession.Query<ArticleRevision>()
                          where revision.ArticleId == articleId.Id
                          orderby revision.CreatedAt descending
                          select new { revision.CreatedAt }).ToList();

            return result.Select(x => new ArticleRevisionDate(x.CreatedAt)).ToList();
        }

        public ArticleRevision GetArticleRevision(ArticleId articleId, ArticleRevisionDate articleRevisionDate)
        {
            string id = (articleId != null) ? articleId.Id : null;

            string articleRevisionId = IdUtility.CreateArticleRevisionId(id, articleRevisionDate);

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

            var result = (from revision in this.CurrentSession.Query<ArticleRevision>()
                          where revision.ArticleId == articleId.Id
                          orderby revision.CreatedAt descending
                          select new { revision.Id, revision.CreatedAt }).ToList();

            return result.ToDictionary(x => x.Id, x => x.CreatedAt);
        }

        public ArticleResult GetArticleResult(string articleId, DateTime? revisionDate = null)
        {
            var articleIdValue = new ArticleId(articleId);

            return this.GetArticleResult(articleIdValue, revisionDate);
        }

        public ArticleResult GetArticleResult(ArticleId articleId, DateTime? revisionDate = null)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }

            EnsureArticleSlug(articleId);

            var result = new ArticleResult();

            var article = this.GetArticle(articleId);
            if (article == null)
            {
                var articleRedirect = this.GetArticleRedirect(articleId);
                if (articleRedirect != null)
                {
                    result.ArticleRedirect = articleRedirect;

                    article = this.GetArticle(articleRedirect.RedirectToArticleSlug);
                }
            }

            result.Article = article;

            string articleRevisionId = GetArticleRevisionId(revisionDate, article);

            result.ArticleRevision = this.GetArticleRevision(articleRevisionId);

            return result;
        }

        private static string GetArticleRevisionId(DateTime? revisionDate, Article article)
        {
            if (revisionDate.HasValue && article != null)
            {
                return IdUtility.CreateArticleRevisionId(article.Id, revisionDate.Value);
            }
            if (article != null)
            {
                return article.ActiveRevisionId;
            }
            return null;
        }

        public ArticleResult SaveArticle(
            Article article,
            ArticleRevision articleRevision = null,
            params ArticleRedirect[] articleRedirects)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            bool isNewArticle = string.IsNullOrWhiteSpace(article.Id);

            if (isNewArticle)
            {
                this.CurrentSession.Store(article);
            }

            this.SaveArticleRevision(article, articleRevision);

            this.SaveArticleRedirectsInternal(article.Id, articleRedirects);

            this.CurrentSession.SaveChanges();

            return new ArticleResult(article, articleRevision);
        }

        public ArticleResult SaveArticleRevision(ArticleId articleId, ArticleRevision articleRevision)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }
            if (articleRevision == null)
            {
                throw new ArgumentNullException("articleRevision");
            }

            var article = this.GetArticle(articleId);
            if (article == null)
            {
                string message = string.Format("No Article found with ID '{0}'.", articleId.Id);
                throw new ArgumentOutOfRangeException("articleId", message);
            }

            return this.SaveArticle(article, articleRevision);
        }

        public IReadOnlyCollection<ArticleRedirect> SaveArticleRedirects(
            ArticleId redirectToArticleId,
            params ArticleRedirect[] articleRedirects)
        {
            if (redirectToArticleId == null)
            {
                throw new ArgumentNullException("redirectToArticleId");
            }

            var savedArticleRedirects = this.SaveArticleRedirectsInternal(redirectToArticleId, articleRedirects);

            this.CurrentSession.SaveChanges();

            return savedArticleRedirects;
        }

        private IReadOnlyCollection<ArticleRedirect> SaveArticleRedirectsInternal(
            ArticleId redirectToArticleId,
            params ArticleRedirect[] articleRedirects)
        {
            if (articleRedirects == null)
            {
                throw new ArgumentNullException("articleRedirects");
            }

            if (!articleRedirects.Any())
            {
                return articleRedirects;
            }

            var articleRedirectIds =
                articleRedirects.Select(x => IdUtility.CreateArticleRedirectId(x.OriginalArticleSlug)).ToList();

            var storedRedirects = this.CurrentSession.Load<ArticleRedirect>(articleRedirectIds);

            var ensuredRedirects = new List<ArticleRedirect>();
            for (int i = 0; i < storedRedirects.Length; i++)
            {
                var redirect = storedRedirects[i] ?? articleRedirects[i];

                this.CurrentSession.Store(redirect);

                ensuredRedirects.Add(redirect);
            }

            var ensuredOriginalArticleSlugs = ensuredRedirects.Select(x => x.OriginalArticleSlug).ToList();

            var deletedRedirects =
                this.CurrentSession.Query<ArticleRedirect>()
                    .Where(
                        x =>
                        x.RedirectToArticleSlug == redirectToArticleId.Slug
                        && !x.OriginalArticleSlug.In(ensuredOriginalArticleSlugs))
                    .ToList();

            deletedRedirects.ForEach(x => this.CurrentSession.Delete(x));

            return ensuredRedirects;
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

        private static void EnsureArticleSlug(ArticleId articleId)
        {
            string ensuredSlug = articleId.Slug;

            if (articleId.OriginalSlug != ensuredSlug)
            {
                throw new ArticleIdNotEnsuredException(articleId.OriginalSlug, ensuredSlug);
            }
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

        public IReadOnlyCollection<ArticleRedirect> DebugAllArticleRedirects()
        {
            using (var session = this.GetMaxRequestsSession())
            {
                return session.Query<ArticleRedirect>().Take(int.MaxValue).ToList();
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