using System;
using System.Collections.Generic;
using System.Web;

using Raven.Client;

namespace WikiDown.Website
{
    public static class DocumentStoreAppInstance
    {
        private const string DocumentStoreKey = "WikiDown_DocumentStore";

        public static IDocumentStore Get(HttpApplicationStateBase application = null)
        {
            application = application ?? TryGetApplication();
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            var documentStore = application[DocumentStoreKey] as IDocumentStore;
            if (documentStore == null)
            {
                throw new KeyNotFoundException("Could not find WikiDown IDocumentStore in Application-state.");
            }

            return documentStore;
        }

        public static void Set(IDocumentStore documentStore, HttpApplicationStateBase application = null)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException("documentStore");
            }

            application = application ?? TryGetApplication();
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            application[DocumentStoreKey] = documentStore;
        }

        private static HttpApplicationStateBase TryGetApplication()
        {
            return (HttpContext.Current != null)
                       ? new HttpApplicationStateWrapper(HttpContext.Current.Application)
                       : null;
        }
    }
}