using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

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

        private readonly IPrincipal principal;

        public Repository(IDocumentStore documentStore, IPrincipal principal)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException("documentStore");
            }
            if (principal == null)
            {
                throw new ArgumentNullException("principal");
            }

            this.documentStore = documentStore;
            this.principal = principal;

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

        public bool DeleteArticle(ArticleId articleId)
        {
            var article = this.GetArticle(articleId);
            if (article == null)
            {
                return false;
            }

            this.CurrentSession.Delete(article);

            var articleRevisions =
                this.CurrentSession.Query<ArticleRevision, ArticleRevisionsIndex>()
                    .Where(x => x.ArticleId == article.Id);
            var articleRedirects =
                this.CurrentSession.Query<ArticleRedirect, ArticleRedirectsIndex>()
                    .Where(x => x.RedirectToArticleSlug == article.Id);
            var articleAccess = this.GetArticleAccess(articleId);

            articleRevisions.ToList().ForEach(x => this.CurrentSession.Delete(x));
            articleRedirects.ToList().ForEach(x => this.CurrentSession.Delete(x));
            this.CurrentSession.Delete(articleAccess);

            this.CurrentSession.SaveChanges();

            return true;
        }

        public bool DeleteArticleRevision(ArticleId articleId, ArticleRevisionDate revisionDate)
        {
            var articleRevision = this.GetArticleRevision(articleId, revisionDate);
            if (articleRevision == null)
            {
                return false;
            }

            this.CurrentSession.Delete(articleRevision);

            var article = this.GetArticle(articleId);
            if (article.ActiveRevisionId == articleRevision.Id)
            {
                var latestRevision =
                    this.CurrentSession.Query<ArticleRevision, ArticleRevisionsIndex>()
                        .OrderByDescending(x => x.CreatedAt)
                        .FirstOrDefault();

                article.ActiveRevisionId = (latestRevision != null) ? latestRevision.Id : null;
            }

            this.CurrentSession.SaveChanges();
            return true;
        }

        public Article GetArticle(ArticleId articleId)
        {
            return (articleId != null && articleId.HasValue) ? this.CurrentSession.Load<Article>(articleId.Id) : null;
        }

        public IReadOnlyCollection<ArticleId> GetArticleDrafts()
        {
            var principalName = this.GetPrincipalIdentityName();
            if (string.IsNullOrWhiteSpace(principalName))
            {
                return Enumerable.Empty<ArticleId>().ToList();
            }

            var accessRole = (int)ArticleAccessHelper.GetRole(this.principal);

            var result =
                (from article in
                     this.CurrentSession.Query<Article, ArticlesListIndex>().AsProjection<ArticlesListIndex.Result>()
                 where
                     accessRole >= article.CanReadRole
                     && (article.ActiveRevisionId == null || article.ActiveRevisionId == string.Empty)
                     && article.CreatedByUserId == principalName
                 orderby article.Title
                 select new { article.Title }).ToList();

            return result.Select(x => new ArticleId(x.Title)).ToList();
        }

        public IReadOnlyCollection<ArticleId> GetArticleList()
        {
            var accessRole = (int)ArticleAccessHelper.GetRole(this.principal);

            var result =
                (from article in
                     this.CurrentSession.Query<Article, ArticlesListIndex>().AsProjection<ArticlesListIndex.Result>()
                 where
                     accessRole >= article.CanReadRole
                     && (article.ActiveRevisionId != null && article.ActiveRevisionId != string.Empty)
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

            var result = (from redirect in this.CurrentSession.Query<ArticleRedirect, ArticleRedirectsIndex>()
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

            var result = (from revision in this.CurrentSession.Query<ArticleRevision, ArticleRevisionsIndex>()
                          where revision.ArticleId == articleId.Id
                          orderby revision.CreatedAt descending
                          select new { revision.CreatedAt }).ToList();

            return result.Select(x => new ArticleRevisionDate(x.CreatedAt)).ToList();
        }

        public ArticleRevision GetArticleRevision(ArticleId articleId, ArticleRevisionDate revisionDate)
        {
            string id = (articleId != null) ? articleId.Id : null;

            string articleRevisionId = IdUtility.CreateArticleRevisionId(id, revisionDate);

            return this.GetArticleRevision(articleRevisionId);
        }

        public ArticleRevision GetArticleRevision(string articleRevisionId)
        {
            return (articleRevisionId != null) ? this.CurrentSession.Load<ArticleRevision>(articleRevisionId) : null;
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

            ArticleRedirect articleRedirect = null;

            var article = this.CurrentSession.Include<Article>(x => x.ActiveRevisionId).Load(articleId.Id);
            if (article == null)
            {
                articleRedirect = this.GetArticleRedirect(articleId);
                if (articleRedirect != null)
                {
                    article = this.GetArticle(articleRedirect.RedirectToArticleSlug);
                }
            }

            string articleRevisionId = revisionDate.HasValue
                                           ? IdUtility.CreateArticleRevisionId(articleId, revisionDate.Value)
                                           : ((article != null) ? article.ActiveRevisionId : null);

            var articleRevision = this.GetArticleRevision(articleRevisionId);

            return new ArticleResult(article, articleRevision, articleRedirect);
        }

        public ArticleResult SaveArticle(
            Article article,
            ArticleRevision articleRevision = null,
            bool publishRevision = true)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            if (string.IsNullOrWhiteSpace(article.Id))
            {
                article.CreatedByUserId = article.CreatedByUserId ?? this.GetPrincipalIdentityName();

                this.CurrentSession.Store(article);
            }

            this.SaveArticleRevisionInternal(article, articleRevision, publishRevision);

            this.CurrentSession.SaveChanges();

            return new ArticleResult(article, articleRevision);
        }

        public ArticleResult SaveArticleRevision(
            ArticleId articleId,
            ArticleRevision articleRevision,
            bool publishRevision = true)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }
            if (articleRevision == null)
            {
                throw new ArgumentNullException("articleRevision");
            }

            var article = this.GetArticle(articleId) ?? new Article(articleId);

            return this.SaveArticle(article, articleRevision, publishRevision);
        }

        public IReadOnlyCollection<ArticleRedirect> SaveArticleRedirects(
            ArticleId redirectToArticleId,
            params string[] articleRedirects)
        {
            if (redirectToArticleId == null)
            {
                throw new ArgumentNullException("redirectToArticleId");
            }

            var redirects = articleRedirects.Select(x => new ArticleRedirect(x, redirectToArticleId)).ToArray();

            return this.SaveArticleRedirects(redirectToArticleId, redirects);
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
                this.CurrentSession.Query<ArticleRedirect, ArticleRedirectsIndex>()
                    .Where(
                        x =>
                        x.RedirectToArticleSlug == redirectToArticleId.Slug
                        && !x.OriginalArticleSlug.In(ensuredOriginalArticleSlugs))
                    .ToList();

            deletedRedirects.ForEach(x => this.CurrentSession.Delete(x));

            return ensuredRedirects;
        }

        public bool PublishArticleRevision(ArticleId articleId, ArticleRevisionDate revisionDate)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }
            if (revisionDate == null)
            {
                throw new ArgumentNullException("revisionDate");
            }

            var article = this.GetArticle(articleId);
            if (article == null)
            {
                return false;
            }

            var articleRevision = this.GetArticleRevision(articleId, revisionDate);
            if (articleRevision == null)
            {
                return false;
            }

            article.ActiveRevisionId = articleRevision.Id;

            this.CurrentSession.SaveChanges();

            return true;
        }

        public bool RevertArticleToDraft(ArticleId articleId)
        {
            if (articleId == null)
            {
                throw new ArgumentNullException("articleId");
            }


            var article = this.GetArticle(articleId);
            if (article == null)
            {
                return false;
            }

            article.ActiveRevisionId = null;

            this.CurrentSession.SaveChanges();

            return true;
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

        private string GetPrincipalIdentityName()
        {
            return (this.principal.Identity != null && !string.IsNullOrWhiteSpace(this.principal.Identity.Name))
                       ? this.principal.Identity.Name
                       : null;
        }

        private IDocumentSession GetMaxRequestsSession()
        {
            var session = this.documentStore.OpenSession();
            session.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;

            return session;
        }

        private void SaveArticleRevisionInternal(Article article, ArticleRevision articleRevision, bool publishRevision)
        {
            if (articleRevision == null || string.IsNullOrWhiteSpace(articleRevision.MarkdownContent))
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

            if (publishRevision)
            {
                article.ActiveRevisionId = articleRevision.Id;
            }
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