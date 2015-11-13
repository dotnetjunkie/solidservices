namespace BusinessLayer.CrossCuttingConcerns
{
    using System.Security;
    using System.Security.Principal;
    using Contract;

    public class AuthorizationQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult> 
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> decoratedHandler;
        private readonly IPrincipal currentUser;
        private readonly ILogger logger;

        public AuthorizationQueryHandlerDecorator(IQueryHandler<TQuery, TResult> decoratedHandler,
            IPrincipal currentUser, ILogger logger)
        {
            this.decoratedHandler = decoratedHandler;
            this.currentUser = currentUser;
            this.logger = logger;
        }

        public TResult Handle(TQuery query)
        {
            this.Authorize();

            return this.decoratedHandler.Handle(query);
        }

        private void Authorize()
        {
            // Some useful authorization logic here.
            if (typeof(TQuery).Namespace.Contains("Admin") && !this.currentUser.IsInRole("Admin"))
            {
                throw new SecurityException();
            }

            this.logger.Log("User " + this.currentUser.Identity.Name + " has been authorized to execute " +
                typeof(TQuery).Name);
        }
    }
}