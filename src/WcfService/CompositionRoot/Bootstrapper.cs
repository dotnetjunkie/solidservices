namespace WcfService.CompositionRoot
{
    using System;
    using System.Reflection;
    using System.Security.Principal;
    using System.Threading;
    using BusinessLayer;
    using Contract;
    using SimpleInjector;
    using WcfService.Code;
    using WcfService.CrossCuttingConcerns;

    public static class Bootstrapper
    {
        private static Container container;

        public static object GetInstance(Type serviceType)
        {
            return container.GetInstance(serviceType);
        }

        public static T GetInstance<T>() where T : class
        {
            return container.GetInstance<T>();
        }
        
        public static void Bootstrap()
        {
            // Did you know the container can diagnose your configuration? Go to: http://bit.ly/YE8OJj.
            container = new Container();

            BusinessLayerBootstrapper.Bootstrap(container);

            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(ToWcfFaultTranslatorCommandHandlerDecorator<>));

            container.RegisterWcfServices(Assembly.GetExecutingAssembly());

            RegisterWcfSpecificDependencies();

            container.Verify();
        }

        private static void RegisterWcfSpecificDependencies()
        {
            container.RegisterSingleton<ILogger, DebugLogger>();

            container.Register<IPrincipal>(() => Thread.CurrentPrincipal);
        }
    }
}