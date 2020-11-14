namespace WcfService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using BusinessLayer;
    using WcfService.Code;
    using WcfService.CrossCuttingConcerns;

    public sealed class Bootstrapper : BusinessLayerBootstrapper
    {
        public static readonly Bootstrapper Instance = new Bootstrapper();

        public Bootstrapper() : base(
            // WCF-specific Singletons
            logger: new DebugLogger(),
            principal: Thread.CurrentPrincipal)
        {
        }

        public static IEnumerable<Type> GetQueryAndResultTypes()
        {
            var queryTypes = BusinessLayerBootstrapper.GetQueryTypes().Select(q => q.QueryType);
            var resultTypes = BusinessLayerBootstrapper.GetQueryTypes().Select(q => q.ResultType).Distinct();
            return queryTypes.Concat(resultTypes);
        }

        protected override ICommandHandler<TCommand> Decorate<TCommand>(
            DbContext context, ICommandHandler<TCommand> handler) =>
            new ToWcfFaultTranslatorCommandHandlerDecorator<TCommand>(
                decoratee: base.Decorate(context, handler));

        public static void Log(Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }
    }
}