namespace BusinessLayer
{
    using BusinessLayer.CrossCuttingConcerns;
    using Contract;
    using SimpleInjector;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    // This class allows registering all types that are defined in the business layer, and are shared across
    // all applications that use this layer (WCF and Web API). For simplicity, this class is placed inside
    // this assembly, but this does couple the business layer assembly to the used container. If this is a 
    // concern, create a specific BusinessLayer.Bootstrap project with this class.
    public static class BusinessLayerBootstrapper
    {
        private static readonly Assembly[] ContractAssemblies = new[] { typeof(IQuery<>).Assembly };
        private static readonly Assembly[] BusinessLayerAssemblies = new[] { Assembly.GetExecutingAssembly() };

        public static void Bootstrap(Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.RegisterInstance<IValidator>(new DataAnnotationsValidator());

            container.Register(typeof(ICommandHandler<>), BusinessLayerAssemblies);
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(ValidationCommandHandlerDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(AuthorizationCommandHandlerDecorator<>));

            container.Register(typeof(IQueryHandler<,>), BusinessLayerAssemblies);
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(ValidationQueryHandlerDecorator<,>));
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(AuthorizationQueryHandlerDecorator<,>));
        }

        public static IEnumerable<Type> GetCommandTypes() =>
            from assembly in ContractAssemblies
            from type in assembly.GetExportedTypes()
            where type.Name.EndsWith("Command")
            select type;

        public static Type CreateQueryHandlerType(Type queryType) =>
            typeof(IQueryHandler<,>).MakeGenericType(queryType, DetermineResultTypes(queryType).Single());

        public static IEnumerable<(Type QueryType, Type ResultType)> GetQueryTypes() =>
            from assembly in ContractAssemblies
            from type in assembly.GetExportedTypes()
            where IsQuery(type)
            select (type, DetermineResultTypes(type).Single());

        public static Type GetQueryResultType(Type queryType) => DetermineResultTypes(queryType).Single();

        private static bool IsQuery(Type type) => DetermineResultTypes(type).Any();

        private static IEnumerable<Type> DetermineResultTypes(Type type) =>
            from interfaceType in type.GetInterfaces()
            where interfaceType.IsGenericType
            where interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)
            select interfaceType.GetGenericArguments()[0];
    }
}