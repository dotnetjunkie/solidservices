namespace WebApiService
{
    using System.Linq;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using System.Web.Http.Description;
    using BusinessLayer;
    using Code;
    using Newtonsoft.Json.Serialization;
    using SimpleInjector;
    using SolidServices.Controllerless.WebApi.Description;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, Container container)
        {
            // Setting the same-origin policy to 'unrestricted'. Remove or change this line if you want to 
            // restrict web pages from making AJAX requests to other domains. For more information, see:
            // https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/enabling-cross-origin-requests-in-web-api#scope-rules-for-enablecors
            config.EnableCors(new EnableCorsAttribute(origins: "*", headers: "*", methods: "*"));

            UseCamelCaseJsonSerialization(config);

#if DEBUG
            UseIndentJsonSerialization(config);
#endif
            MapRoutes(config, container);

            UseControllerlessApiDocumentation(config);
        }

        private static void MapRoutes(HttpConfiguration config, Container container)
        {
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

        private static void UseControllerlessApiDocumentation(HttpConfiguration config)
        {
            var queryApiExplorer = new ControllerlessApiExplorer(
                messageTypes: Bootstrapper.GetKnownQueryTypes().Select(t => t.QueryType),
                responseTypeSelector: type => BusinessLayerBootstrapper.GetQueryResultType(type))
            {
                ControllerName = "queries",
                ParameterSourceSelector = type => ApiParameterSource.FromUri,
                HttpMethodSelector = type => HttpMethod.Get,
                ActionNameSelector = type => type.Name.Replace("Query", string.Empty)
            };

            var commandApiExplorer = new ControllerlessApiExplorer(
                messageTypes: Bootstrapper.GetKnownCommandTypes(),
                responseTypeSelector: type => typeof(void))
            {
                ControllerName = "commands",
                ParameterName = "command",
                ActionNameSelector = type => type.Name.Replace("Command", string.Empty),
            };

            config.Services.Replace(typeof(IApiExplorer),
                new CompositeApiExplorer(
                    config.Services.GetApiExplorer(),
                    commandApiExplorer,
                    queryApiExplorer));
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