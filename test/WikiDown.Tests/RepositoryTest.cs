using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WikiDown.Tests
{
    [TestClass]
    public class RepositoryTest
    {
        private static readonly string TestRedirectArticleId = ArticleId.CreateArticleId("TestRedirectArticleId");

        private const string TestArticleSlug = "TestArticleSlug";

        [TestMethod]
        public void GetArticlePage_WithArticle_ShouldReturnArticleView()
        {
            var article = new Article(TestArticleSlug);
            var articleRevision = new ArticleRevision(string.Empty) { ArticleId = article.Id };

            var repository = RepositoryTestHelper.GetMockRepository(article, articleRevision);
            var articlePage = repository.GetArticlePage(TestArticleSlug);

            Assert.IsNotNull(articlePage);
        }

        [TestMethod]
        public void GetArticlePage_WithArticleWithRedirectToArticle_ShouldReturnPageWithRedirect()
        {
            var testArticleId = new ArticleId(TestRedirectArticleId);

            var article = new Article(TestArticleSlug); //{ RedirectToArticleId = testArticleId };

            var repository = RepositoryTestHelper.GetMockRepository(article);
            
            var articlePage = repository.GetArticlePage(TestArticleSlug);

            Assert.AreEqual(testArticleId.Slug, articlePage.RedirectArticleSlug);
        }

        [TestMethod]
        public void GetArticlePage_WithoutArticle_ShouldReturnPageWithoutArticle()
        {
            var repository = RepositoryTestHelper.GetMockRepository();

            var articlePage = repository.GetArticlePage(TestArticleSlug);

            Assert.IsFalse(articlePage.HasArticle);
        }

        [TestMethod]
        public void GetArticlePage_ArticleWithoutArticleRevision_ShouldReturnPageWithNullRevision()
        {
            var article = new Article(TestArticleSlug);

            var repository = RepositoryTestHelper.GetMockRepository(article);

            var articlePage = repository.GetArticlePage(TestArticleSlug);
            
            Assert.IsNull(articlePage.Revision);
        }

        [TestMethod]
        public void GetArticlePage_WithSlugContainingSpace_ShouldReturnPageWithRedirect()
        {
            var repository = RepositoryTestHelper.GetMockRepository();

            string urlContainingSpace = "URL WITH SPACE";

            var articlePage = repository.GetArticlePage(urlContainingSpace);
            
            var expectedEnsuredId =  new ArticleId(urlContainingSpace);

            Assert.AreEqual(expectedEnsuredId.Slug, articlePage.RedirectArticleSlug);
        }
    }
}