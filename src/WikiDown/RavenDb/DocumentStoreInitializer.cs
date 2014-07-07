using System.Configuration;

using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;

namespace WikiDown.RavenDb
{
    public static class DocumentStoreInitializer
    {
        public static IDocumentStore FromAppSettingName(string appSettingName)
        {
            string appSettingConnectionString = ConfigurationManager.AppSettings[appSettingName];

            var connectionStringParser =
                ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(appSettingConnectionString);

            return GetDocumentStore(connectionStringParser);
        }

        public static IDocumentStore FromConnectionString(string connectionString)
        {
            var connectionStringParser =
                ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(connectionString);

            return GetDocumentStore(connectionStringParser);
        }

        public static IDocumentStore FromConnectionStringName(string connectionStringName)
        {
            var connectionStringParser =
                ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionStringName(connectionStringName);

            return GetDocumentStore(connectionStringParser);
        }

        internal static void InitDocumentStore(IDocumentStore documentStore)
        {
            IdConvensionsHelper.Register(documentStore);

            IndexesHelper.Create(documentStore);
        }

        private static IDocumentStore GetDocumentStore(
            ConnectionStringParser<RavenConnectionStringOptions> connectionStringParser)
        {
            connectionStringParser.Parse();

            string connectionStringApiKey = connectionStringParser.ConnectionStringOptions.ApiKey;
            string connectionStringUrl = connectionStringParser.ConnectionStringOptions.Url;

            var documentStore = new DocumentStore { ApiKey = connectionStringApiKey, Url = connectionStringUrl };
            documentStore.Initialize();

            InitDocumentStore(documentStore);

            return documentStore;
        }
    }
}