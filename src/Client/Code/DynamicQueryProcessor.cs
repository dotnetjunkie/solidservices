namespace Client.Code
{
    using System;
    using System.Diagnostics;
    using Contract;

    public sealed class DynamicQueryProcessor : IQueryProcessor
    {
        [DebuggerStepThrough]
        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(WcfServiceQueryHandlerProxy<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = Activator.CreateInstance(handlerType);

            return handler.Handle((dynamic)query);
        }
    }
}