using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq;

using WikiDown.Markdown;
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

        public ArticleId GetArticleExists(ArticleId articleId)
        {
            var article = this.GetArticle(articleId);
            if (article != null)
            {
                return article.Id;
            }

            var redirect = this.GetArticleRedirect(articleId);
            if (redirect != null)
            {
                return redirect.RedirectToArticleSlug;
            }

            return null;
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
                          orderby redirect.RedirectFromArticleSlug
                          select redirect).As<ArticleRedirect>().ToList();

            return result.Select(x => new ArticleId(x.RedirectFromArticleSlug)).ToList();
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

            var result =
                query.Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(15)))
                    .AsProjection<ArticleRevisionItem>();
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

        public IReadOnlyCollection<ArticleSearchResultItem> SearchArticleTitles(string searchTerm)
        {
            searchTerm = (searchTerm ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<ArticleSearchResultItem>(0);
            }

            var searchTermWildcard = GetArticlesSearchTermWildcard(searchTerm);

            var query =
                this.CurrentSession.Query<SearchArticlesIndex.Result, SearchArticlesIndex>()
                    .Where(
                        x =>
                        x.Slug.StartsWith(searchTermWildcard, StringComparison.InvariantCultureIgnoreCase)
                        && this.principalAccessLevel.Value >= x.CanReadAccess)
                    .As<ArticleSearchResultItem>();

            return GetArticlesSearchResults(query, searchTerm).ToList();
        }

        public ArticleSearchResult SearchArticles(string searchTerm)
        {
            searchTerm = (searchTerm ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new ArticleSearchResult(Enumerable.Empty<ArticleSearchResultItem>(), null);
            }

            var searchTermWildcard = GetArticlesSearchTermWildcard(searchTerm);

            var query =
                this.CurrentSession.Query<SearchArticlesIndex.Result, SearchArticlesIndex>()
                    .Customize(
                        x =>
                        x.Highlight("TextContent", SearchArticlesIndex.HighlightFragmentLength, 1, "Highlightings")
                            .SetHighlighterTags("<mark>", "</mark>"))
                    .Search(
                        x => x.Slug,
                        searchTermWildcard,
                        escapeQueryOptions: EscapeQueryOptions.AllowPostfixWildcard)
                    .Search(x => x.TextContent, searchTerm, 0.75M)
                    .Search(x => x.Tags, searchTerm, 0.5M)
                    .Where(x => this.principalAccessLevel.Value >= x.CanReadAccess)
                    .As<ArticleSearchResultItem>();

            var results = GetArticlesSearchResults(query, searchTerm).ToList();

            var suggestionResults = !results.Any() ? query.Suggest() : null;
            var suggestions = (suggestionResults != null) ? suggestionResults.Suggestions : null;

            return new ArticleSearchResult(results, suggestions);
        }

        public void Dispose()
        {
            if (this.currentSessionLazy.IsValueCreated && this.currentSessionLazy.Value != null)
            {
                this.CurrentSession.SaveChanges();

                this.currentSessionLazy.Value.Dispose();
            }

            if (this.currentAsyncSessionLazy.IsValueCreated && this.currentAsyncSessionLazy.Value != null)
            {
                var saveTask = this.currentAsyncSessionLazy.Value.SaveChangesAsync();
                saveTask.Wait();

                this.currentAsyncSessionLazy.Value.Dispose();
            }
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

        private IRavenQueryable<ArticlesIndex.Result> GetArticlesQueryBase()
        {
            return from article in this.CurrentSession.Query<ArticlesIndex.Result, ArticlesIndex>()
                   where this.principalAccessLevel.Value >= article.CanReadAccess
                   orderby article.ArticleSlug
                   select article;
        }

        private static string GetArticlesSearchTermWildcard(string searchTerm)
        {
            return searchTerm.TrimEnd('*').Trim() + '*';
        }

        private static IEnumerable<ArticleSearchResultItem> GetArticlesSearchResults(
            IEnumerable<ArticleSearchResultItem> query,
            string searchTerm)
        {
            var result =
                query.GroupBy(x => x.RedirectToSlug ?? x.Slug)
                    .Select(x => x.First())
                    .OrderByDescending(
                        x =>
                        (x.RedirectToSlug ?? x.Slug ?? string.Empty).StartsWith(
                            searchTerm,
                            StringComparison.InvariantCultureIgnoreCase));

            var test = result.ToList();

            return result.ToList();
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
            var article = this.GetArticle(articleRedirect.RedirectFromArticleSlug);
            return (article != null);
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

            var articleRedirectIds = redirects.Select(IdUtility.CreateArticleRedirectId).ToList();

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

            var ensuredOriginalArticleSlugs = ensuredRedirects.Select(x => x.RedirectFromArticleSlug).ToList();

            var deletedRedirects =
                this.CurrentSession.Query<ArticleRedirect, ArticleRedirectsIndex>()
                    .Where(
                        x =>
                        x.RedirectToArticleSlug == redirectToArticleId.Slug
                        && !x.RedirectFromArticleSlug.In(ensuredOriginalArticleSlugs))
                    .ToList();

            deletedRedirects.ForEach(x => this.CurrentSession.Delete(x));

            return ensuredRedirects.OrderBy(x => x.RedirectFromArticleSlug).ToList();
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

        public IReadOnlyCollection<Article> DebugSaveAllArticles()
        {
            using (var session = this.GetMaxRequestsSession())
            {
                var articles = session.Query<Article>().Take(int.MaxValue).ToList();

                foreach (var article in articles)
                {
                    session.Store(article);
                }

                session.SaveChanges();
                return articles;
            }
        }

        public IReadOnlyCollection<ArticleRevision> DebugSaveAllArticleRevisions()
        {
            using (var session = this.GetMaxRequestsSession())
            {
                var query =
                    session.Query<ArticleRevisionsIndex.Result, ArticleRevisionsIndex>()
                        .Where(x => x.IsActive)
                        .Take(int.MaxValue);

                var articleRevisions = query.As<ArticleRevision>().ToList();

                foreach (var revision in articleRevisions)
                {
                    revision.TextContent = MarkdownService.MakeTextFlat(revision.MarkdownContent);

                    session.Store(revision);
                }

                session.SaveChanges();
                return articleRevisions;
            }
        }

        public IReadOnlyCollection<ArticleRedirect> DebugSaveAllArticleRedirects()
        {
            using (var session = this.GetMaxRequestsSession())
            {
                var query = session.Query<ArticleRedirect, ArticleRedirectsIndex>().Take(int.MaxValue);

                var articleRevisions = query.ToList();

                foreach (var revision in articleRevisions)
                {
                    session.Store(revision);
                }

                session.SaveChanges();
                return articleRevisions;
            }
        }
#endif
    }
}