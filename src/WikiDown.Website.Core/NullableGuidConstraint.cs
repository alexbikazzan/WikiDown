using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace WikiDown.Website
{
    public class NullableGuidConstraint : IRouteConstraint, IHttpRouteConstraint
    {
        public bool Match(
            HttpRequestMessage request,
            IHttpRoute route,
            string parameterName,
            IDictionary<string, object> values,
            HttpRouteDirection routeDirection)
        {
            return MatchInternal(parameterName, values);
        }

        public bool Match(
            HttpContextBase httpContext,
            Route route,
            string parameterName,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            return MatchInternal(parameterName, values);
        }

        private static bool MatchInternal(string parameterName, IDictionary<string, object> values)
        {
            if (parameterName == null)
            {
                throw new ArgumentNullException("parameterName");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            object value;
            if (!values.TryGetValue(parameterName, out value))
            {
                return false;
            }

            if (value is Guid)
            {
                return true;
            }

            string stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);

            Guid guid;
            return string.IsNullOrWhiteSpace(stringValue) || Guid.TryParse(stringValue, out guid);
        }
    }
}