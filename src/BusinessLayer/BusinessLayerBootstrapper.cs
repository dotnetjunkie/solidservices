namespace BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using BusinessLayer.CommandHandlers;
    using BusinessLayer.CrossCuttingConcerns;
    using BusinessLayer.QueryHandlers;
    using Contract;
    using Contract.Commands.Orders;
    using Contract.Queries.Orders;

    // This class allows registering all types that are defined in the business layer, and are shared across
    // all applications that use this layer (WCF and Web API). For simplicity, this class is placed inside
    // this assembly, but this does couple the business layer assembly to the used container. If this is a 
    // concern, create a specific BusinessLayer.Bootstrap project with this class.
    public abstract class BusinessLayerBootstrapper
    {
        private static Assembly[] contractAssemblies = new[] { typeof(IQuery<>).Assembly };

        // Singletons
        private readonly IValidator validator = new DataAnnotationsValidator();
        private readonly ILogger logger;
        private readonly IPrincipal principal;

        public BusinessLayerBootstrapper(ILogger logger, IPrincipal principal)
        {
            this.logger = logger;
            this.principal = principal;
        }

        public object GetCommandHandler(Type commandType)
        {
            // Create Scoped components
            var context = new DbContext();

            // Create Transient components
            switch (commandType.Name)
            {
                case nameof(CreateOrderCommand):
                    return this.Decorate(context, new CreateOrderCommandHandler(this.logger));

                case nameof(ShipOrderCommand):
                    return this.Decorate(context, new ShipOrderCommandHandler(this.logger));

                // ==> TODO: This is where you will add you new command handlers

                default:
                    throw new InvalidOperationException($"Unknown command type {commandType}.");
            }
        }

        protected virtual ICommandHandler<TCommand> Decorate<TCommand>(
            DbContext context, ICommandHandler<TCommand> handler) =>
            new AuthorizationCommandHandlerDecorator<TCommand>(
                new ValidationCommandHandlerDecorator<TCommand>(
                    this.validator,
                    handler),
                this.principal,
                this.logger);

        public object GetQueryHandler(Type queryType)
        {
            // Create Scoped components
            var context = new DbContext();

            // Create Transient components
            switch (queryType.Name)
            {
                case nameof(GetOrderByIdQuery):
                    return this.Decorate(context, new GetOrderByIdQueryHandler());

                case nameof(GetUnshippedOrdersForCurrentCustomerQuery):
                    return this.Decorate(context, new GetUnshippedOrdersForCurrentCustomerQueryHandler(this.logger));

                // ==> TODO: This is where you will add you new query handlers

                default:
                    throw new InvalidOperationException($"Unknown query type {queryType}.");
            }
        }

        private IQueryHandler<TQuery, TResult> Decorate<TQuery, TResult>(
            DbContext context, IQueryHandler<TQuery, TResult> handler)
            where TQuery : IQuery<TResult> =>
            new AuthorizationQueryHandlerDecorator<TQuery, TResult>(
                new ValidationQueryHandlerDecorator<TQuery, TResult>(
                    this.validator,
                    handler),
                this.principal,
                this.logger);

        // Verifies the defined mappings in the CreateCommandHandler and CreateQueryHandler methods.
        public void Verify()
        {
            this.VerifyCommandHandlers();
            this.VerifyQueryHandlers();
        }

        private void VerifyCommandHandlers()
        {
            var invalidCommandHandler = (
                from commandType in BusinessLayerBootstrapper.GetCommandTypes()
                let actualHandler = this.GetCommandHandler(commandType)
                let expectedHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType)
                where !expectedHandlerType.IsAssignableFrom(actualHandler.GetType())
                select new { commandType, expectedHandlerType, actualHandler })
                .FirstOrDefault();

            if (invalidCommandHandler != null)
            {
                throw new InvalidOperationException(
                    $"Handler {invalidCommandHandler.actualHandler} was returned for command " +
                    $"{invalidCommandHandler.commandType} which doesn't implement " +
                    $"{invalidCommandHandler.expectedHandlerType}.");
            }
        }

        private void VerifyQueryHandlers()
        {
            var invalidQueryHandler = (
                from info in BusinessLayerBootstrapper.GetQueryTypes()
                let actualHandler = this.GetQueryHandler(info.QueryType)
                let expectedHandlerType = typeof(IQueryHandler<,>).MakeGenericType(info.QueryType, info.ResultType)
                where !expectedHandlerType.IsAssignableFrom(actualHandler.GetType())
                select new { info, expectedHandlerType, actualHandler })
                .FirstOrDefault();

            if (invalidQueryHandler != null)
            {
                throw new InvalidOperationException(
                    $"Handler {invalidQueryHandler.actualHandler} was returned for query " +
                    $"{invalidQueryHandler.info.QueryType} which doesn't implement " +
                    $"{invalidQueryHandler.expectedHandlerType}.");
            }
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

    public sealed class DbContext { }

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