namespace Client.Controllers
{
    using System;
    using Contract;
    using Contract.Commands.Orders;
    using Contract.DTOs;

    public class CommandExampleController
    {
        private readonly ICommandHandler<CreateOrderCommand> createOrderhandler;
        private readonly ICommandHandler<ShipOrderCommand> shipOrderhandler;

        public CommandExampleController(
            ICommandHandler<CreateOrderCommand> createOrderhandler,
            ICommandHandler<ShipOrderCommand> shipOrderhandler)
        {
            this.createOrderhandler = createOrderhandler;
            this.shipOrderhandler = shipOrderhandler;
        }

        public Guid CreateOrder()
        {
            var createOrderCommand = new CreateOrderCommand
            {
                ShippingAddress = new Address
                {
                    Country = "The Netherlands",
                    City = "Nijmegen",
                    Street = ".NET Street"
                }
            };

            this.createOrderhandler.Handle(createOrderCommand);

            Console.WriteLine("Order with ID {0} has been created.", createOrderCommand.CreatedOrderId);

            return createOrderCommand.CreatedOrderId;
        }

        public void ShipOrder(Guid orderId)
        {
            this.shipOrderhandler.Handle(new ShipOrderCommand { OrderId = orderId });

            Console.WriteLine("Order with ID {0} is shipped.", orderId);
        }
    }
}