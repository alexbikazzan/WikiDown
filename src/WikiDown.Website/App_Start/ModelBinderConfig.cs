using System;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.ModelBinding.Binders;
using System.Web.Mvc;

using WikiDown.Website.ModelBinding;

using WebApi = System.Web.Http;

namespace WikiDown.Website
{
    public static class ModelBinderConfig
    {
        public static void RegisterModelBinders(ModelBinderDictionary modelBinders, HttpConfiguration config)
        {
            modelBinders.Add(typeof(ArticleId), new ArticleIdModelBinder());
            modelBinders.Add(typeof(ArticleRevisionDate), new ArticleRevisionDateModelBinder());
            modelBinders.Add(typeof(bool?), new BooleanModelBinder());
            modelBinders.Add(typeof(bool), new BooleanModelBinder());

            AddWebApiModelBinder(config, typeof(ArticleId), new ArticleIdModelBinder());
            AddWebApiModelBinder(config, typeof(ArticleRevisionDate), new ArticleRevisionDateModelBinder());
        }

        private static void AddWebApiModelBinder(
            HttpConfiguration config,
            Type modelType,
            WebApi.ModelBinding.IModelBinder modelBinder)
        {
            var modelBinderProvider = new SimpleModelBinderProvider(modelType, modelBinder);
            config.Services.Insert(typeof(ModelBinderProvider), 0, modelBinderProvider);
        }
    }
}