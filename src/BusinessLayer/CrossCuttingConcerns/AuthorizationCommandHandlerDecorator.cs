namespace BusinessLayer.CrossCuttingConcerns
{
    using System.Security;
    using System.Security.Principal;

    using Contract;

    public class AuthorizationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> decoratedHandler;
        private readonly IPrincipal currentUser;
        private readonly ILogger logger;

        public AuthorizationCommandHandlerDecorator(ICommandHandler<TCommand> decoratedHandler,
            IPrincipal currentUser, ILogger logger)
        {
            this.decoratedHandler = decoratedHandler;
            this.currentUser = currentUser;
            this.logger = logger;
        }

        public void Handle(TCommand query)
        {
            this.Authorize();

            this.decoratedHandler.Handle(query);
        }

        private void Authorize()
        {
            // Some useful authorization logic here.
            if (typeof(TCommand).Namespace.Contains("Admin") && !this.currentUser.IsInRole("Admin"))
            {
                throw new SecurityException();
            }

            this.logger.Log("User " + this.currentUser.Identity.Name + " has been authorized to execute " + 
                typeof(TCommand).Name);
        }
    }
}