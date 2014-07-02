using Raven.Client.Embedded;

using WikiDown.RavenDb;

namespace WikiDown.Tests
{
    public static class RepositoryTestHelper
    {
        public static Repository GetMockRepository(Article article = null, ArticleRevision articleRevision = null)
        {
            var documentStore =
                new EmbeddableDocumentStore
                    {
                        Configuration =
                            {
                                RunInUnreliableYetFastModeThatIsNotSuitableForProduction
                                    = true,
                                DefaultStorageTypeName = "munin",
                                RunInMemory = true,
                            }
                    }.Initialize();

            DocumentStoreInitializer.InitDocumentStore(documentStore);

            var repository = new Repository(documentStore);

            if (article != null)
            {
                repository.SaveArticle(article, articleRevision);
            }

            return new Repository(documentStore);
        }
    }
}