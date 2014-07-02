using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;

using Moq;
using WikiDown.Website.Controllers;

namespace WikiDown.Website.Tests
{
    public static class RoutingTestHelper
    {
        public static UrlHelper GetUrlHelper(RouteCollection routeCollection = null)
        {
            routeCollection = routeCollection ?? GetRegisteredRoutes();

            return GetMockedUrlHelper(routeCollection);
        }

        public static RouteCollection GetRegisteredRoutes()
        {
            var routeCollection = new RouteCollection();

            RouteConfig.RegisterRoutes(routeCollection, RegisterAutomaticRoutes);

            return routeCollection;
        }

        private static UrlHelper GetMockedUrlHelper(RouteCollection routeCollection)
        {
            var mockRequest = new Mock<HttpRequestBase>();
            var mockHttpContext = new Mock<HttpContextBase>();

            mockHttpContext.Setup(x => x.Request).Returns(mockRequest.Object);

            var requestContext = new RequestContext(mockHttpContext.Object, new RouteData());

            return (routeCollection != null)
                       ? new UrlHelper(requestContext, routeCollection)
                       : new UrlHelper(requestContext);
        }

        private static void RegisterAutomaticRoutes(
            RouteCollection routeCollection,
            IInlineConstraintResolver constraintResolver)
        {
            var controllerAssembly = typeof(WikiController).Assembly;
            routeCollection.MapMvcAttributeRoutes(controllerAssembly, constraintResolver);
        }
    }
}