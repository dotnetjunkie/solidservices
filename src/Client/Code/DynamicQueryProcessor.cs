namespace Client.Code
{
    using System.Diagnostics;
    using SimpleInjector;
    using Contract;

    public sealed class DynamicQueryProcessor : IQueryProcessor
    {
        private readonly Container container;

        public DynamicQueryProcessor(Container container)
        {
            this.container = container;
        }

        [DebuggerStepThrough]
        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = this.container.GetInstance(handlerType);

            return handler.Handle((dynamic)query);
        }
    }
}