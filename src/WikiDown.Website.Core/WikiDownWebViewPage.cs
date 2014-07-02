using System;
using System.Web.WebPages;

using AspNetSeo;

namespace WikiDown.Website
{
    public class WikiDownWebViewPage : WikiDownWebViewPage<dynamic>
    {
    }

    public class WikiDownWebViewPage<TModel> : SeoWebViewPage<TModel>
    {
        private static readonly object EmptyObject = new object();

        public override void Execute()
        {
        }

        public HelperResult RenderSection(string name, Func<object, HelperResult> defaultContent)
        {
            return this.IsSectionDefined(name) ? this.RenderSection(name) : defaultContent(EmptyObject);
        }
    }
}