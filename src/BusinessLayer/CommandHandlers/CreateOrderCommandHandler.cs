namespace BusinessLayer.CommandHandlers
{
    using System;

    using Contract;
    using Contract.Commands.Orders;

    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand>
    {
        private readonly ILogger logger;

        public CreateOrderCommandHandler(ILogger logger)
        {
            this.logger = logger;
        }

        public void Handle(CreateOrderCommand command)
        {
            // Do something useful here.
            command.CreatedOrderId = Guid.NewGuid();

            this.logger.Log(this.GetType().Name + " has been executed successfully.");
        }
    }
}