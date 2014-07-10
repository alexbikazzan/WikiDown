using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

using AspNetSeo;
using Microsoft.Owin;
using Owin;
using Raven.Client;
using WikiDown.RavenDb;
using WikiDown.Website.Security;

[assembly: OwinStartupAttribute(typeof(WikiDown.Website.Startup))]

namespace WikiDown.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static IDocumentStore DocumentStore { get; private set; }

        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            ModelBinderConfig.RegisterModelBinders(ModelBinders.Binders, GlobalConfiguration.Configuration);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles();

            DocumentStore = DocumentStoreInitializer.FromAppSettingName("RavenDbConnectionString");

            var application = new HttpApplicationStateWrapper(this.Application);
            DocumentStoreAppInstance.Set(DocumentStore, application);

            SeoHelper.BaseTitle = "WikiDown";
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AuthConfig.Configure(app);

            RootUserUtility.EnsureRootAccount(MvcApplication.DocumentStore);
        }
    }
}