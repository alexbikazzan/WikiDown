using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WikiDown.Tests
{
    [TestClass]
    public class ArticleIdTest
    {
        private const string Rfc3986CharsPattern =
            @"^[ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789\-._~:/?#\[\]@!$&'\(\)\*\+,;=%]+$";

        private static readonly Regex Rfc3986Regex = new Regex(Rfc3986CharsPattern);

        [TestMethod]
        public void CreateArticleId_WithSwedishCharacters_ShouldEncode()
        {
            var articleSlug = "TestArticleTitleÅäö";

            string id = IdUtility.CreateArticleId(articleSlug);
            string idSuffix = GetIdSuffix(id);

            bool matches3986 = Rfc3986Regex.IsMatch(idSuffix);
            Assert.IsTrue(matches3986);
        }

        [TestMethod]
        public void CreateArticleId_WithSpaceCharacter_ShouldEncode()
        {
            var articleSlug = "TestArticleTitle Test";

            string id = IdUtility.CreateArticleId(articleSlug);
            string idSuffix = GetIdSuffix(id);

            bool matches3986 = Rfc3986Regex.IsMatch(idSuffix);
            Assert.IsTrue(matches3986);
        }

        private static string GetIdSuffix(string id)
        {
            int slashIndex = id.LastIndexOf('/');
            return (slashIndex >= 0) ? id.Substring(slashIndex + 1) : id;
        }
    }
}