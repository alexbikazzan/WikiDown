using System;
using System.Collections;
using System.Security.Principal;
using System.Web.Http.Controllers;
using System.Web.Routing;

using Raven.Client;

namespace WikiDown.Website
{
    public static class RepositoryRequestInstance
    {
        private const string RepositoryKey = "WikiDown_Repository";

        public static Repository Get(RequestContext requestContext, IDocumentStore documentStore = null)
        {
            EnsureRequestContext(requestContext);

            documentStore = documentStore ?? DocumentStoreAppInstance.Get();

            return GetRepository(documentStore, requestContext.HttpContext.Items, requestContext.HttpContext.User);
        }

        public static Repository Get(HttpRequestContext requestContext, IDocumentStore documentStore = null)
        {
            EnsureRequestContext(requestContext);

            documentStore = documentStore ?? DocumentStoreAppInstance.Get();

            return GetRepository(documentStore, requestContext.Configuration.Properties, requestContext.Principal);
        }

        public static void TryDispose(RequestContext requestContext)
        {
            EnsureRequestContext(requestContext);

            TryDisposeRepository(requestContext.HttpContext.Items);
        }

        public static void TryDispose(HttpRequestContext requestContext)
        {
            EnsureRequestContext(requestContext);

            TryDisposeRepository(requestContext.Configuration.Properties);
        }

        private static Repository GetRepository(IDocumentStore documentStore, IDictionary store, IPrincipal principal)
        {
            var repository = store[RepositoryKey] as Repository;
            if (repository == null)
            {
                repository = new Repository(documentStore, principal);
                store[RepositoryKey] = repository;
            }

            return repository;
        }

        private static void EnsureRequestContext(RequestContext requestContext)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }
            if (requestContext.HttpContext == null)
            {
                throw new NullReferenceException("HttpContext is null on RequestContext.");
            }
            if (requestContext.HttpContext.Items == null)
            {
                throw new NullReferenceException("HttpContext-items is null on RequestContext.");
            }
        }

        private static void EnsureRequestContext(HttpRequestContext requestContext)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException("requestContext");
            }
            if (requestContext.Configuration == null)
            {
                throw new NullReferenceException("Configuration is null on RequestContext.");
            }
            if (requestContext.Configuration.Properties == null)
            {
                throw new NullReferenceException("Configuration-properties is null on RequestContext.");
            }
        }

        private static void TryDisposeRepository(IDictionary dictionary)
        {
            var repository = dictionary[RepositoryKey] as Repository;
            if (repository == null)
            {
                return;
            }

            repository.Dispose();

            dictionary.Remove(RepositoryKey);
        }
    }
}