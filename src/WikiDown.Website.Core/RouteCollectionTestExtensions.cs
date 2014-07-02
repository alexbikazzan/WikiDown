using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;

namespace WikiDown.Website
{
    public static class RouteCollectionTestExtensions
    {
        public static void MapMvcAttributeRoutes(
            this RouteCollection routeCollection,
            Assembly controllerAssembly,
            IInlineConstraintResolver constraintResolver = null)
        {
            var controllerTypes = (from type in controllerAssembly.GetExportedTypes()
                                   where
                                       type != null && type.IsPublic
                                       && type.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                                       && !type.IsAbstract && typeof(IController).IsAssignableFrom(type)
                                   select type).ToList();

            var attributeRoutingAssembly = typeof(RouteCollectionAttributeRoutingExtensions).Assembly;
            var attributeRoutingMapperType =
                attributeRoutingAssembly.GetType("System.Web.Mvc.Routing.AttributeRoutingMapper");

            var mapAttributeRoutesMethod = attributeRoutingMapperType.GetMethod(
                "MapAttributeRoutes",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(RouteCollection), typeof(IEnumerable<Type>), typeof(IInlineConstraintResolver) },
                null);

            constraintResolver = constraintResolver ?? new DefaultInlineConstraintResolver();
            mapAttributeRoutesMethod.Invoke(null, new object[] { routeCollection, controllerTypes, constraintResolver });
        }
    }
}