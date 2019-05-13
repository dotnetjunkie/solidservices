namespace BusinessLayer.CrossCuttingConcerns
{
    using System;
    using Contract;

    public class ValidationQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IValidator validator;
        private readonly IQueryHandler<TQuery, TResult> handler;

        public ValidationQueryHandlerDecorator(IValidator validator, IQueryHandler<TQuery, TResult> handler)
        {
            this.validator = validator;
            this.handler = handler;
        }

        TResult IQueryHandler<TQuery, TResult>.Handle(TQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            // validate the supplied command.
            this.validator.ValidateObject(query);

            // forward the (valid) command to the real command handler.
            return this.handler.Handle(query);
        }
    }
}