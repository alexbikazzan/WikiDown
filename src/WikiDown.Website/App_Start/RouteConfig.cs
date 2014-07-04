using System;
using System.Web.Mvc.Routing;
using System.Web.Mvc;
using System.Web.Routing;

namespace WikiDown.Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            RegisterRoutes(routes, RegisterAutomaticRoutes);
        }

        internal static void RegisterRoutes(
            RouteCollection routes,
            Action<RouteCollection, IInlineConstraintResolver> registerAutomaticRoutesFactory)
        {
            routes.AppendTrailingSlash = true;
            routes.RouteExistingFiles = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var constraintResolver = new DefaultInlineConstraintResolver();
            constraintResolver.ConstraintMap.Add("guid?", typeof(NullableGuidConstraint));

            registerAutomaticRoutesFactory(routes, constraintResolver);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }

        private static void RegisterAutomaticRoutes(
            RouteCollection routes,
            IInlineConstraintResolver constraintResolver)
        {
            AreaRegistration.RegisterAllAreas();

            routes.MapMvcAttributeRoutes(constraintResolver);
        }
    }
}