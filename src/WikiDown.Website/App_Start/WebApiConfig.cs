using System.Web.Http;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace WikiDown.Website
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Formatters.Remove(config.Formatters.FormUrlEncodedFormatter);
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            var formatting = (DevEnvironment.IsDebug) ? Formatting.Indented : Formatting.None;
            var contractResolver = new CamelCasePropertyNamesContractResolver();
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fff" };

            var jsonSerializerSettings = config.Formatters.JsonFormatter.SerializerSettings;
            
            jsonSerializerSettings.Formatting = formatting;
            jsonSerializerSettings.ContractResolver = contractResolver;
            jsonSerializerSettings.Converters.Add(dateTimeConverter);
            
            config.IncludeErrorDetailPolicy = DevEnvironment.IsDebug
                                                  ? IncludeErrorDetailPolicy.Always
                                                  : IncludeErrorDetailPolicy.Default;

            config.MapHttpAttributeRoutes();

            config.EnsureInitialized();
        }
    }
}