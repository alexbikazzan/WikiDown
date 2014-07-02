using System.Web.Mvc;

namespace WikiDown.Website
{
    public static class ModelBinderConfig
    {
        public static void RegisterModelBinders(ModelBinderDictionary modelBinders)
        {
            modelBinders.Add(typeof(ArticleRevisionDate), new ArticleRevisionDateModelBinder());
        }
    }
}