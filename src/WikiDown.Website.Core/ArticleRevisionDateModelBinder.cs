using System;
using System.Web.Mvc;

namespace WikiDown.Website
{
    public class ArticleRevisionDateModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };

            object model = null;
            try
            {
                string attemptedValue = ((valueResult != null) ? valueResult.AttemptedValue : null) ?? string.Empty;

                model = !string.IsNullOrWhiteSpace(attemptedValue)
                            ? new ArticleRevisionDate(attemptedValue)
                            : ArticleRevisionDate.Empty;
            }
            catch (FormatException ex)
            {
                modelState.Errors.Add(ex);
            }

            bindingContext.ModelState.Add(bindingContext.ModelName, modelState);

            return model;
        }
    }
}