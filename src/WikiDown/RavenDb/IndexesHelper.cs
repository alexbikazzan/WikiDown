using System.Linq;
using System.Threading.Tasks;

using Raven.Client;
using Raven.Client.Indexes;
using WikiDown.RavenDb.Indexes;

namespace WikiDown.RavenDb
{
    internal static class IndexesHelper
    {
        public static void Create(IDocumentStore documentStore)
        {
            var existingIndexes = documentStore.DatabaseCommands.GetIndexNames(0, int.MaxValue);

            var indexesAssembly = typeof(SearchArticlesIndex).Assembly;

            var wikiDownIndexes =
                indexesAssembly.GetTypes()
                    .Where(x => x.IsSubclassOf(typeof(AbstractIndexCreationTask)))
                    .Select(x => x.Name);

            var createIndexesTask = Task.Run(() => IndexCreation.CreateIndexes(indexesAssembly, documentStore));
            
            bool hasAllIndexes = wikiDownIndexes.All(existingIndexes.Contains);
            if (!hasAllIndexes)
            {
                createIndexesTask.Wait();
            }
        }
    }
}