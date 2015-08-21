namespace WebApiService.CompositionRoot
{
    using System;
    using System.Reflection;
    using System.Security.Principal;
    using System.Threading;
    using System.Web;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;
    using BusinessLayer;
    using Contract;
    using SimpleInjector;
    using SimpleInjector.Integration.Web;
    using WebApiService.Code;

    public static class Bootstrapper
    {
        public static void Bootstrap()
        {
            // Did you know the container can diagnose your configuration? Go to: https://bit.ly/YE8OJj.
            var container = new Container();

            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            BusinessLayerBootstrapper.Bootstrap(container);

            RegisterWebApiSpecificDependencies(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.RegisterMvcIntegratedFilterProvider();

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = 
                new SimpleInjectorWebApiDependencyResolver(container);

            System.Web.Mvc.DependencyResolver.SetResolver(
                new SimpleInjector.Integration.Web.Mvc.SimpleInjectorDependencyResolver(container));
        }

        private static void RegisterWebApiSpecificDependencies(Container container)
        {
            container.RegisterSingleton<IPrincipal>(new HttpContextPrincipal());
            container.RegisterSingleton<ILogger, DebugLogger>();
            container.RegisterSingleton<IQueryProcessor, DynamicQueryProcessor>();

            // This provider builds the list of commands and queries.
            var provider = new CommandControllerDescriptorProvider(typeof(ICommandHandler<>).Assembly);

            container.RegisterSingleton<CommandControllerDescriptorProvider>(provider);
           
            container.RegisterSingleton<IHttpControllerSelector, CommandHttpControllerSelector>();
            container.RegisterSingleton<IHttpActionSelector, CommandHttpActionSelector>();

            // This line is optional, but by registering all controllers explicitly, they will be
            // verified when calling Verify().
            foreach (var commandDescriptor in provider.GetDescriptors())
            {
                container.Register(commandDescriptor.ControllerDescriptor.ControllerType);
            }
        }

        private sealed class HttpContextPrincipal : IPrincipal
        {
            private IPrincipal Principal
            {
                get { return HttpContext.Current.User ?? Thread.CurrentPrincipal; }
            }

            public IIdentity Identity
            {
                get { return this.Principal.Identity; }
            }

            public bool IsInRole(string role)
            {
                return this.Principal.IsInRole(role);
            }
        }
    }
}