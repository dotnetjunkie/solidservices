namespace Client.CompositionRoot
{
    using Client.Code;
    using Client.CrossCuttingConcerns;
    using Contract;
    using SimpleInjector;
    using SimpleInjector.Extensions;

    public static class Bootstrapper
    {
        private static Container container;

        public static void Bootstrap()
        {
            container = new Container();

            container.RegisterSingle<IQueryProcessor, DynamicQueryProcessor>();

            container.RegisterOpenGeneric(typeof(ICommandHandler<>), typeof(WcfServiceCommandHandlerProxy<>));
            container.RegisterOpenGeneric(typeof(IQueryHandler<,>), typeof(WcfServiceQueryHandlerProxy<,>));

            container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(FromWcfFaultTranslatorCommandHandlerDecorator<>));

            container.Verify();
        }

        public static TService GetInstance<TService>() where TService : class
        {
            return container.GetInstance<TService>();
        }
    }
}