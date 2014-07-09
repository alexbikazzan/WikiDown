using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WikiDown.Website
{
    public class EnsureSlugAttribute : ActionFilterAttribute
    {
        private const string DefaultRouteValueName = "slug";

        private readonly string routeValueName;

        public EnsureSlugAttribute()
        {
            this.routeValueName = DefaultRouteValueName;
        }

        public EnsureSlugAttribute(string routeValueName)
        {
            this.routeValueName = routeValueName;
        }

        public string RouteName { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            base.OnActionExecuting(filterContext);

            var requestUrl = filterContext.HttpContext.Request.Url;
            if (requestUrl == null)
            {
                return;
            }

            var queryString = GetQueryString(requestUrl);

            var ensuredUrl = this.GetEnsuredUrl(filterContext);
            var ensuredPathAndQuery = GetEnsuredPathAndQuery(ensuredUrl, queryString);

            var decodedEnsured = HttpUtility.UrlDecode(ensuredPathAndQuery);
            var decodedRequestUrl = HttpUtility.UrlDecode(requestUrl.PathAndQuery);

            if (decodedEnsured == decodedRequestUrl)
            {
                return;
            }

            filterContext.Result = new RedirectResult(ensuredUrl, true /*permanent*/);
        }

        private static string GetEnsuredPathAndQuery(string ensuredUrl, Dictionary<string, string> requestQueryString)
        {
            var ensuredQueryStringIndex = ensuredUrl.IndexOf('?');
            var ensuredQueryStringValue = (ensuredQueryStringIndex >= 0)
                                              ? ensuredUrl.Substring(ensuredQueryStringIndex)
                                              : null;
            var ensuredQueryString = GetQueryString(ensuredQueryStringValue ?? string.Empty);

            var extraQueryStringKeys = requestQueryString.Keys.Where(x => !ensuredQueryString.Keys.Contains(x));
            var extraQueryStringValues =
                extraQueryStringKeys.Select(
                    x =>
                    string.Format("{0}={1}", HttpUtility.UrlEncode(x), HttpUtility.UrlEncode(requestQueryString[x])));

            var extraQueryString = string.Join("&", extraQueryStringValues);

            string separator = !string.IsNullOrWhiteSpace(extraQueryString)
                                   ? (ensuredUrl.Contains("?") ? "&" : "?")
                                   : null;

            return string.Format("{0}{1}{2}", ensuredUrl.TrimEnd('&'), separator, extraQueryString);
        }

        private string GetEnsuredUrl(ControllerContext filterContext)
        {
            var routeValues = new RouteValueDictionary(filterContext.RouteData.Values);
            var slugValue = (routeValues[routeValueName] as string) ?? string.Empty;

            var ensuredSlugValue = ArticleSlugUtility.Decode(slugValue);
            ensuredSlugValue = ArticleSlugUtility.Encode(ensuredSlugValue);

            routeValues[routeValueName] = ensuredSlugValue;

            var urlHelper = new UrlHelper(filterContext.RequestContext);
            return (!string.IsNullOrWhiteSpace(this.RouteName))
                       ? urlHelper.RouteUrl(this.RouteName, routeValues)
                       : urlHelper.RouteUrl(routeValues);
        }

        private static Dictionary<string, string> GetQueryString(Uri requestUri)
        {
            string queryString = (requestUri != null) ? requestUri.Query : string.Empty;
            return GetQueryString(queryString);
        }

        private static Dictionary<string, string> GetQueryString(string queryString)
        {
            var queryStringValues = HttpUtility.ParseQueryString(queryString);

            return queryStringValues.AllKeys.ToDictionary(x => x, x => queryStringValues[x]);
        }
    }
}