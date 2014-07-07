using System.Net;

using Mvc = System.Web.Mvc;
using WebApi = System.Web.Http;

namespace WikiDown.Website.ModelBinding
{
    public class ArticleIdModelBinder : Mvc.DefaultModelBinder, WebApi.ModelBinding.IModelBinder
    {
        public override object BindModel(
            Mvc.ControllerContext controllerContext,
            Mvc.ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            var modelState = new Mvc.ModelState { Value = valueProviderResult };

            var value = (valueProviderResult != null) ? valueProviderResult.AttemptedValue : null;

            var model = new ArticleId(value ?? string.Empty);

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

            return model;
        }

        public bool BindModel(
            WebApi.Controllers.HttpActionContext actionContext,
            WebApi.ModelBinding.ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(ArticleId))
            {
                return false;
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            string value = (valueProviderResult != null) ? valueProviderResult.AttemptedValue : null;

            if (value == null)
            {
                throw new WebApi.HttpResponseException(HttpStatusCode.BadRequest);
            }

            bindingContext.Model = new ArticleId(value);
            return true;
        }
    }
}