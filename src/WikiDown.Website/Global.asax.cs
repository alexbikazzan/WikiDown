using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

using AspNetSeo;
using Raven.Client;
using WikiDown.RavenDb;

namespace WikiDown.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static IDocumentStore DocumentStore { get; private set; }

        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            ModelBinderConfig.RegisterModelBinders(ModelBinders.Binders);
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            BundleConfig.RegisterBundles();

            DocumentStore = DocumentStoreInitializer.FromAppSettingName("RavenDbConnectionString");

            SeoHelper.BaseTitle = "WikiDown";
        }
    }
}