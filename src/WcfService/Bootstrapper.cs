namespace WcfService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using System.Threading;
    using BusinessLayer;
    using SimpleInjector;
    using WcfService.Code;
    using WcfService.CrossCuttingConcerns;

    public static class Bootstrapper
    {
        private static Container container;

        public static object GetCommandHandler(Type commandType) =>
            container.GetInstance(typeof(ICommandHandler<>).MakeGenericType(commandType));

        public static object GetQueryHandler(Type queryType) =>
            container.GetInstance(BusinessLayerBootstrapper.CreateQueryHandlerType(queryType));

        public static IEnumerable<Type> GetCommandTypes() => BusinessLayerBootstrapper.GetCommandTypes();

        public static IEnumerable<Type> GetQueryAndResultTypes()
        {
            var queryTypes = BusinessLayerBootstrapper.GetQueryTypes().Select(q => q.QueryType);
            var resultTypes = BusinessLayerBootstrapper.GetQueryTypes().Select(q => q.ResultType).Distinct();
            return queryTypes.Concat(resultTypes);
        }

        public static void Bootstrap()
        {
            container = new Container();

            BusinessLayerBootstrapper.Bootstrap(container);

            container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(ToWcfFaultTranslatorCommandHandlerDecorator<>));

            container.RegisterWcfServices(Assembly.GetExecutingAssembly());

            RegisterWcfSpecificDependencies();

            container.Verify();
        }

        public static void Log(Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        private static void RegisterWcfSpecificDependencies()
        {
            container.RegisterInstance<ILogger>(new DebugLogger());

            container.Register<IPrincipal>(() => Thread.CurrentPrincipal);
        }
    }
}