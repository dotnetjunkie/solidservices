namespace BusinessLayer.CrossCuttingConcerns
{
    using Contract;
    using System.Diagnostics;

    public sealed class StructuredLoggingQueryHandlerDecorator<TQuery, TResult>
        : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly StructuredMessageLogger<TQuery> logger;
        private readonly IQueryHandler<TQuery, TResult> decoratee;

        public StructuredLoggingQueryHandlerDecorator(
            StructuredMessageLogger<TQuery> logger, IQueryHandler<TQuery, TResult> decoratee)
        {
            this.logger = logger;
            this.decoratee = decoratee;
        }

        public TResult Handle(TQuery query)
        {
            var watch = Stopwatch.StartNew();

            var result = this.decoratee.Handle(query);
            
            this.logger.Log(query, watch.Elapsed);

            return result;
        }
    }
}