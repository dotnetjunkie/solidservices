namespace Client
{
    using Client.Code;
    using Client.Controllers;
    using Client.CrossCuttingConcerns;
    using Contract;
    using SimpleInjector;

    public static class Bootstrapper
    {
        private static Container container;

        public static void Bootstrap()
        {
            container = new Container();

            container.RegisterInstance<IQueryProcessor>(new DynamicQueryProcessor(container));

            container.Register(typeof(ICommandHandler<>), typeof(WcfServiceCommandHandlerProxy<>));
            container.Register(typeof(IQueryHandler<,>), typeof(WcfServiceQueryHandlerProxy<,>));

            container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(FromWcfFaultTranslatorCommandHandlerDecorator<>));

            container.Register<CommandExampleController>();
            container.Register<QueryExampleController>();

            container.Verify();
        }

        public static TService GetInstance<TService>() where TService : class
        {
            return container.GetInstance<TService>();
        }
    }
}