namespace BusinessLayer.CrossCuttingConcerns
{
    using System;
    using Contract;

    public class ValidationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly IValidator validator;
        private readonly ICommandHandler<TCommand> handler;

        public ValidationCommandHandlerDecorator(IValidator validator, ICommandHandler<TCommand> handler)
        {
            this.validator = validator;
            this.handler = handler;
        }

        void ICommandHandler<TCommand>.Handle(TCommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            // validate the supplied command.
            this.validator.ValidateObject(command);

            // forward the (valid) command to the real command handler.
            this.handler.Handle(command);
        }
    }
}