namespace BusinessLayer.CrossCuttingConcerns
{
    using System.Diagnostics;

    public sealed class StructuredLoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly StructuredMessageLogger<TCommand> logger;
        private readonly ICommandHandler<TCommand> decoratee;

        public StructuredLoggingCommandHandlerDecorator(
            StructuredMessageLogger<TCommand> logger, ICommandHandler<TCommand> decoratee)
        {
            this.logger = logger;
            this.decoratee = decoratee;
        }

        public void Handle(TCommand command)
        {
            var watch = Stopwatch.StartNew();

            this.decoratee.Handle(command);

            this.logger.Log(command, watch.Elapsed);
        }
    }
}