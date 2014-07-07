using System;

using Microsoft.AspNet.Identity;
using Raven.Client;

namespace WikiDown.Website.Security
{
    public static class UserManagerHelper
    {
        public static Lazy<UserManager<WikiDownUser>> GetLazy(IDocumentStore documentStore)
        {
            return new Lazy<UserManager<WikiDownUser>>(
                () =>
                    {
                        var session = documentStore.OpenAsyncSession();
                        session.Advanced.UseOptimisticConcurrency = true;

                        var userStore = new WikiDownRavenUserStore(session);
                        return new UserManager<WikiDownUser>(userStore);
                    });
        }
    }
}