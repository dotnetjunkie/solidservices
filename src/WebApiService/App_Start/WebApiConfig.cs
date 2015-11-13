namespace WebApiService
{
    using System.Web.Http;
    using Code;
    using SimpleInjector;

    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, Container container)
        {
            config.Routes.MapHttpRoute(
                name: "CommandApi",
                routeTemplate: "api/commands/{command}",
                defaults: new { },
                constraints: new { },
                handler: new CommandDelegatingHandler(
                    serviceLocator: container.GetInstance,
                    commandTypes: Bootstrapper.GetKnownCommandTypes()));

            config.Routes.MapHttpRoute(
                name: "QueryApi",
                routeTemplate: "api/queries/{query}",
                defaults: new { },
                constraints: new { },
                handler: new QueryDelegatingHandler(
                    serviceLocator: container.GetInstance,
                    queryTypes: Bootstrapper.GetKnownQueryTypes()));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional });
        }
    }
}