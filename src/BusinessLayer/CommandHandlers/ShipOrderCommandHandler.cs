namespace BusinessLayer.CommandHandlers
{
    using Contract;
    using Contract.Commands.Orders;

    public class ShipOrderCommandHandler : ICommandHandler<ShipOrderCommand>
    {
        private readonly ILogger logger;

        public ShipOrderCommandHandler(ILogger logger)
        {
            this.logger = logger;
        }

        public void Handle(ShipOrderCommand command)
        {
            this.logger.Log("Shipping order " + command.OrderId);
        }
    }
}