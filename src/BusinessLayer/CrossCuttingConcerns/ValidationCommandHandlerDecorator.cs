namespace BusinessLayer.CrossCuttingConcerns
{
    using Contract;

    public class ValidationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly IValidator validator;
        private readonly ILogger logger;

        public ValidationCommandHandlerDecorator(ICommandHandler<TCommand> handler, 
            IValidator validator, ILogger logger)
        {
            this.handler = handler;
            this.validator = validator;
            this.logger = logger;
        }

        void ICommandHandler<TCommand>.Handle(TCommand command)
        {
            // validate the supplied command.
            this.validator.ValidateObject(command);

            this.logger.Log(typeof(TCommand).Name + " is valid.");

            // forward the (valid) command to the real command handler.
            this.handler.Handle(command);
        }
    }
}
