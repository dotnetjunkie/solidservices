namespace BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using BusinessLayer.CrossCuttingConcerns;
    using Contract;
    using SimpleInjector;

    // This class allows registering all types that are defined in the business layer, and are shared across
    // all applications that use this layer (WCF and Web API). For simplicity, this class is placed inside
    // this assembly, but this does couple the business layer assembly to the used container. If this is a 
    // concern, create a specific BusinessLayer.Bootstrap project with this class.
    public static class BusinessLayerBootstrapper
    {
        private static Assembly[] contractAssemblies = new[] { typeof(IQuery<>).Assembly };
        private static Assembly[] businessLayerAssemblies = new[] { Assembly.GetExecutingAssembly() };

        public static void Bootstrap(Container container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.RegisterSingleton<IValidator>(new DataAnnotationsValidator(container));
            
            container.Register(typeof(ICommandHandler<>), businessLayerAssemblies);
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(ValidationCommandHandlerDecorator<>));
            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(AuthorizationCommandHandlerDecorator<>));

            container.Register(typeof(IQueryHandler<,>), businessLayerAssemblies);
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(AuthorizationQueryHandlerDecorator<,>));
        }
        
        public static IEnumerable<Type> GetCommandTypes() =>
            from assembly in contractAssemblies
            from type in assembly.GetExportedTypes()
            where type.Name.EndsWith("Command")
            select type;

        public static IEnumerable<QueryInfo> GetQueryTypes() =>
            from assembly in contractAssemblies
            from type in assembly.GetExportedTypes()
            where QueryInfo.IsQuery(type)
            select new QueryInfo(type);
    }

    [DebuggerDisplay("{QueryType.Name,nq}")]
    public sealed class QueryInfo
    {
        public readonly Type QueryType;
        public readonly Type ResultType;

        public QueryInfo(Type queryType)
        {
            this.QueryType = queryType;
            this.ResultType = DetermineResultTypes(queryType).Single();
        }

        public static bool IsQuery(Type type) => DetermineResultTypes(type).Any();

        private static IEnumerable<Type> DetermineResultTypes(Type type) =>
            from interfaceType in type.GetInterfaces()
            where interfaceType.IsGenericType
            where interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)
            select interfaceType.GetGenericArguments()[0];
    }
}