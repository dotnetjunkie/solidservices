namespace WebApiService
{
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using SimpleInjector.Integration.WebApi;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            var container = Bootstrapper.Bootstrap();

            WebApiConfig.Register(GlobalConfiguration.Configuration, container);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }
    }
}