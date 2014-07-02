using System.Web.Mvc;

using AspNetSeo;

namespace WikiDown.Website
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleExceptionsAttribute());
            filters.Add(new SeoModelFilterAttribute());
        }
    }
}