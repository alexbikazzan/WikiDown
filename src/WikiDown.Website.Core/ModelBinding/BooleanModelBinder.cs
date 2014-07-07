using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace WikiDown.Website.ModelBinding
{
    public class BooleanModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            bool isTypeBool = GetIsTypeBoolean(bindingContext.ModelType);
            if (isTypeBool)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

                bool? value = GetBooleanValue(valueProviderResult);
                if (value.HasValue)
                {
                    return value.Value;
                }
            }

            return base.BindModel(controllerContext, bindingContext);
        }

        protected override void BindProperty(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor)
        {
            bool isTypeBool = GetIsTypeBoolean(propertyDescriptor.PropertyType);
            if (isTypeBool)
            {
                var valueProviderResult = bindingContext.ValueProvider.GetValue(propertyDescriptor.Name);

                bool? value = GetBooleanValue(valueProviderResult);
                if (value.HasValue)
                {
                    propertyDescriptor.SetValue(bindingContext.Model, value.Value);
                }
            }

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }

        private static bool GetIsTypeBoolean(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        private static bool? GetBooleanValue(ValueProviderResult valueProviderResult)
        {
            string attemptedValue = ((valueProviderResult != null) ? valueProviderResult.AttemptedValue : null)
                                    ?? string.Empty;

            switch (attemptedValue)
            {
                case "0":
                    return false;
                case "1":
                    return true;
                default:
                    return null;
            }
        }
    }
}