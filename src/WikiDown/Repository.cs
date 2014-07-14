using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

using Raven.Client;
using Raven.Client.Indexes;
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

        private readonly string principalId;

        private readonly Lazy<int> principalAccessLevel;

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

            this.principalId = (principal.Identity != null) ? principal.Identity.Name : null;
            this.principalAccessLevel = new Lazy<int>(() => (int)principal.GetAccessLevel());

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

            articleRevisions.ToList().ForEach(x => this.CurrentSession.Delete(x));
            articleRedirects.ToList().ForEach(x => this.CurrentSession.Delete(x));

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

        public IReadOnlyCollection<ArticleId> GetArticleDraftsList()
        {
            if (string.IsNullOrWhiteSpace(this.principalId))
            {
                return Enumerable.Empty<ArticleId>().ToList();
            }

            var baseQuery = this.GetArticlesQueryBase();

            var result =
                baseQuery.Where(x => x.ActiveRevisionId == null || x.ActiveRevisionId == string.Empty)
                    .As<Article>()
                    .Select(x => x.Title)
                    .ToList();

            return result.Select(x => new ArticleId(x)).ToList();
        }

        //public IReadOnlyCollection<ArticleId> GetArticleList()
        //{
        //    var query = this.GetArticlesQuery();

        //    var result = query.As<Article>().Select(x => x.Title).ToList();

        //    return result.Select(x => new ArticleId(x)).ToList();
        //}

        //private IRavenQueryable<ArticlesIndex.Result> GetArticlesQuery()
        //{
        //    var baseQuery = this.GetArticlesQueryBase();

        //    return baseQuery.Where(x => x.ActiveRevisionId != null && x.ActiveRevisionId != string.Empty);
        //}

        public ArticleRedirect GetArticleRedirect(ArticleId originalArticleId)
        {
            string articleRedirectId = IdUtility.CreateArticleRedirectId(originalArticleId);

            return this.CurrentSession.Load<ArticleRedirect>(articleRedirectId);
        }

        public IReadOnlyCollection<ArticleId> GetArticleRedirectsList(ArticleId redirectToArticleId)
        {
            if (redirectToArticleId == null || !redirectToArticleId.HasValue)
            {
                return Enumerable.Empty<ArticleId>().ToList();
            }

            var result = (from redirect in this.CurrentSession.Query<ArticleRedirect, ArticleRedirectsIndex>()
                          where redirect.RedirectToArticleSlug == redirectToArticleId.Slug
                          select redirect).As<ArticleRedirect>().ToList();

            return result.Select(x => new ArticleId(x.OriginalArticleSlug)).ToList();
        }

        public IReadOnlyCollection<ArticleRevisionItem> GetArticleRevisionsLatestChangesList()
        {
            var query = this.GetArticleRevisionsQuery<ArticleRevisionsChangedIndex>();

            var result =
                query.Where(x => x.ActiveRevisionId != null && x.ActiveRevisionId != string.Empty)
                    .As<ArticleRevisionItem>();
            return result.ToList();
        }

        public IReadOnlyCollection<ArticleRevisionItem> GetArticleRevisionsList(ArticleId articleId)
        {
            if (articleId == null || !articleId.HasValue)
            {
                return Enumerable.Empty<ArticleRevisionItem>().ToList();
            }

            var query = this.GetArticleRevisionsQuery<ArticleRevisionsIndex>(articleId);

            var result = query.AsProjection<ArticleRevisionItem>();
            return result.ToList();
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

        public ArticleRevision GetArticleRevisionLatest(ArticleId articleId)
        {
            if (articleId == null || !articleId.HasValue)
            {
                return null;
            }

            var query = this.GetArticleRevisionsQuery<ArticleRevisionsIndex>(articleId);

            var result = query.OrderByDescending(x => x.CreatedAt);
            return result.As<ArticleRevision>().FirstOrDefault();
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

            articleRevision.LastPublishedAt = DateTime.UtcNow;
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

        public ArticleResult SaveArticle(
            Article article,
            ArticleRevision articleRevision = null,
            bool publishRevision = false)
        {
            if (article == null)
            {
                throw new ArgumentNullException("article");
            }

            if (string.IsNullOrWhiteSpace(article.Id))
            {
                this.CurrentSession.Store(article);
            }

            this.SaveArticleRevisionInternal(article, articleRevision, publishRevision);

            this.RemoveArticleRedirect(article);

            this.CurrentSession.SaveChanges();

            return new ArticleResult(article, articleRevision);
        }

        public ArticleResult SaveArticleRevision(
            ArticleId articleId,
            ArticleRevision articleRevision,
            bool publishRevision = false)
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
            IEnumerable<string> articleRedirects)
        {
            if (redirectToArticleId == null)
            {
                throw new ArgumentNullException("redirectToArticleId");
            }

            var normalizedRedirects = GetNormalizedCollection(articleRedirects);

            var redirects = normalizedRedirects.Select(x => new ArticleRedirect(x, redirectToArticleId));

            return this.SaveArticleRedirects(redirectToArticleId, redirects);
        }

        public IReadOnlyCollection<ArticleRedirect> SaveArticleRedirects(
            ArticleId redirectToArticleId,
            IEnumerable<ArticleRedirect> articleRedirects)
        {
            if (redirectToArticleId == null)
            {
                throw new ArgumentNullException("redirectToArticleId");
            }
            if (articleRedirects == null)
            {
                throw new ArgumentNullException("articleRedirects");
            }

            var savedArticleRedirects = this.SaveArticleRedirectsInternal(redirectToArticleId, articleRedirects);

            this.CurrentSession.SaveChanges();

            return savedArticleRedirects;
        }

        public IReadOnlyCollection<string> SaveArticleTags(ArticleId articleId, IEnumerable<string> tags)
        {
            var normalizedTags = GetNormalizedCollection(tags);

            var article = this.GetArticle(articleId) ?? new Article(articleId);
            article.Tags = normalizedTags;

            this.CurrentSession.SaveChanges();

            return normalizedTags;
        }

        public dynamic SearchArticles(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (this.currentSessionLazy.IsValueCreated && this.currentSessionLazy.Value != null)
            {
                this.currentSessionLazy.Value.Dispose();
            }

            if (this.currentAsyncSessionLazy.IsValueCreated && this.currentAsyncSessionLazy.Value != null)
            {
                this.currentAsyncSessionLazy.Value.Dispose();
            }
        }

        private IDocumentSession GetMaxRequestsSession()
        {
            var session = this.documentStore.OpenSession();
            session.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;

            return session;
        }

        private static IReadOnlyCollection<string> GetNormalizedCollection(IEnumerable<string> values)
        {
            return
                (values ?? Enumerable.Empty<string>()).Select(x => (x ?? string.Empty).Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();
        }

        private bool GetRedirectExistsAsArticle(ArticleRedirect articleRedirect)
        {
            var article = this.GetArticle(articleRedirect.RedirectToArticleSlug);
            return (article != null);
        }

        private IRavenQueryable<ArticlesIndex.Result> GetArticlesQueryBase()
        {
            return from article in this.CurrentSession.Query<ArticlesIndex.Result, ArticlesIndex>()
                   where principalAccessLevel.Value >= article.CanReadAccess
                   orderby article.ArticleTitle
                   select article;
        }

        private IRavenQueryable<ArticleRevisionsIndex.Result> GetArticleRevisionsQuery<TIndexCreator>(
            ArticleId articleId = null) where TIndexCreator : AbstractIndexCreationTask, new()
        {
            var query = from revision in this.CurrentSession.Query<ArticleRevisionsIndex.Result, TIndexCreator>()
                        where (revision.LastPublishedAt != null || revision.CreatedByUserName == this.principalId)
                        orderby revision.CreatedAt descending
                        select revision;

            if (articleId != null && articleId.HasValue)
            {
                query = query.Where(x => x.ArticleId == articleId.Id);
            }

            return query;
        }

        private void RemoveArticleRedirect(Article article)
        {
            var articleRedirect = this.GetArticleRedirect(article.Id);
            if (articleRedirect != null)
            {
                this.CurrentSession.Delete(articleRedirect);
            }
        }

        private IReadOnlyCollection<ArticleRedirect> SaveArticleRedirectsInternal(
            ArticleId redirectToArticleId,
            IEnumerable<ArticleRedirect> articleRedirects)
        {
            if (articleRedirects == null)
            {
                throw new ArgumentNullException("articleRedirects");
            }

            var redirects = articleRedirects.ToList();

            if (!redirects.Any())
            {
                return redirects;
            }

            var articleRedirectIds =
                redirects.Select(x => IdUtility.CreateArticleRedirectId(x.OriginalArticleSlug)).ToList();

            var storedRedirects = this.CurrentSession.Load<ArticleRedirect>(articleRedirectIds);

            var ensuredRedirects = new List<ArticleRedirect>();
            for (int i = 0; i < storedRedirects.Length; i++)
            {
                var redirect = storedRedirects[i] ?? redirects[i];

                bool redirectExistsAsArticle = GetRedirectExistsAsArticle(redirect);
                if (redirectExistsAsArticle)
                {
                    continue;
                }

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

        private void SaveArticleRevisionInternal(Article article, ArticleRevision articleRevision, bool publishRevision)
        {
            if (articleRevision == null || string.IsNullOrWhiteSpace(articleRevision.MarkdownContent))
            {
                return;
            }

            var activeRevision = this.GetArticleRevision(article.ActiveRevisionId);
            if (activeRevision != null && activeRevision.MarkdownContent == articleRevision.MarkdownContent)
            {
                return;
            }

            articleRevision.ArticleId = article.Id;

            this.CurrentSession.Store(articleRevision);

            if (publishRevision)
            {
                article.ActiveRevisionId = articleRevision.Id;
                articleRevision.LastPublishedAt = DateTime.UtcNow;
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