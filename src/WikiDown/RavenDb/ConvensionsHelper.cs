using System;
using System.Threading.Tasks;

using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Connection.Async;

namespace WikiDown.RavenDb
{
    internal static class ConvensionsHelper
    {
        public static void Register(IDocumentStore documentStore)
        {
            RegisterConvention<Article>(documentStore, ArticleId.CreateArticleId);

            RegisterConvention<ArticleRevision>(documentStore, ArticleId.CreateArticleRevisionId);
        }

        private static void RegisterConvention<TEntity>(
            IDocumentStore documentStore,
            Func<TEntity, string> conventionFactory)
        {
            Func<string, IAsyncDatabaseCommands, TEntity, Task<string>> asyncConvention =
                (dbName, commands, entity) => new Task<string>(() => conventionFactory(entity));

            Func<string, IDatabaseCommands, TEntity, string> convention =
                (dbName, commands, entity) => conventionFactory(entity);

            var conventions = documentStore.Conventions;
            conventions.RegisterAsyncIdConvention(asyncConvention);
            conventions.RegisterIdConvention(convention);
        }
    }
}