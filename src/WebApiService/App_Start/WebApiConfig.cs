namespace WebApiService
{
    using System.Web.Http;
    using Newtonsoft.Json.Serialization;
    using SimpleInjector;
    using Code;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, Container container)
        {
            UseCamelCaseJsonSerialization(config);

#if DEBUG
            UseIndentJsonSerialization(config);
#endif

            config.Routes.MapHttpRoute(
                name: "QueryApi",
                routeTemplate: "api/queries/{query}",
                defaults: new { },
                constraints: new { },
                handler: new QueryDelegatingHandler(
                    handlerFactory: container.GetInstance,
                    queryTypes: Bootstrapper.GetKnownQueryTypes()));

            config.Routes.MapHttpRoute(
                name: "CommandApi",
                routeTemplate: "api/commands/{command}",
                defaults: new { },
                constraints: new { },
                handler: new CommandDelegatingHandler(
                    handlerFactory: container.GetInstance,
                    commandTypes: Bootstrapper.GetKnownCommandTypes()));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }

        private static void UseCamelCaseJsonSerialization(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
        }

        private static void UseIndentJsonSerialization(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.Indent = true;
        }
    }
}