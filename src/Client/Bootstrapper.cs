namespace Client
{
    using Client.Code;
    using Client.Controllers;
    using Client.CrossCuttingConcerns;
    using Contract.Commands.Orders;

    public static class Bootstrapper
    {
        public static QueryExampleController GetQueryExampleController() =>
            new QueryExampleController(
                queryProcessor: new DynamicQueryProcessor());

        public static CommandExampleController GetCommandExampleController() =>
            new CommandExampleController(
                createOrderhandler: GetHandler<CreateOrderCommand>(),
                shipOrderhandler: GetHandler<ShipOrderCommand>());

        private static ICommandHandler<T> GetHandler<T>() =>
            Decorate(new WcfServiceCommandHandlerProxy<T>());

        private static ICommandHandler<T> Decorate<T>(ICommandHandler<T> handler) =>
            new FromWcfFaultTranslatorCommandHandlerDecorator<T>(
                handler);
    }
}