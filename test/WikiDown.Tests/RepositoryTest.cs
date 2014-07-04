using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WikiDown.Tests
{
    [TestClass]
    public class RepositoryTest
    {
        private static readonly ArticleId TestRedirectArticleId = new ArticleId("TestRedirectArticleId");

        private static readonly ArticleId TestArticleId = new ArticleId("TestArticleSlug");

        [TestMethod]
        public void GetArticleResult_WithArticle_ShouldReturnArticleView()
        {
            var article = new Article(TestArticleId);
            var articleRevision = new ArticleRevision(string.Empty) { ArticleId = article.Id };

            var repository = RepositoryTestHelper.GetMockRepository(article, articleRevision);
            var articleResult = repository.GetArticleResult(TestArticleId);

            Assert.IsNotNull(articleResult);
        }

        [TestMethod]
        public void GetArticleResult_WithArticleWithRedirectToArticle_ShouldReturnPageWithRedirect()
        {
            var articleRedirect = new ArticleRedirect(TestArticleId, TestRedirectArticleId);

            var repository = RepositoryTestHelper.GetMockRepository(articleRedirect: articleRedirect);

            var articleResult = repository.GetArticleResult(TestArticleId);

            Assert.IsTrue(articleResult.HasRedirect);
            Assert.AreEqual(TestArticleId.Slug, articleResult.ArticleRedirect.OriginalArticleSlug);
            Assert.AreEqual(TestRedirectArticleId.Slug, articleResult.ArticleRedirectId.Slug);
        }

        [TestMethod]
        public void GetArticleResult_WithoutArticle_ShouldReturnPageWithoutArticle()
        {
            var repository = RepositoryTestHelper.GetMockRepository();

            var articleResult = repository.GetArticleResult(TestArticleId);

            Assert.IsFalse(articleResult.HasArticle);
        }

        [TestMethod]
        public void GetArticleResult_ArticleWithoutArticleRevision_ShouldReturnPageWithNullRevision()
        {
            var article = new Article(TestArticleId);

            var repository = RepositoryTestHelper.GetMockRepository(article);

            var articleResult = repository.GetArticleResult(TestArticleId);

            Assert.IsNull(articleResult.ArticleRevision);
        }

        [TestMethod]
        public void GetArticleResult_WithSlugContainingSpace_ShouldThrowArticleIdNotEnsuredException()
        {
            var repository = RepositoryTestHelper.GetMockRepository();

            string urlContainingSpace = "URL WITH SPACE";

            string exceptionEnsuredSlug = null;
            string exceptionOriginalSlug = null;
            try
            {
                repository.GetArticleResult(urlContainingSpace);
            }
            catch (ArticleIdNotEnsuredException ex)
            {
                exceptionEnsuredSlug = ex.EnsuredSlug;
                exceptionOriginalSlug = ex.OriginalSlug;
            }

            var expectedEnsuredId = new ArticleId(urlContainingSpace);

            Assert.AreEqual(expectedEnsuredId.Slug, exceptionEnsuredSlug);
            Assert.AreEqual(urlContainingSpace, exceptionOriginalSlug);
        }
    }
}