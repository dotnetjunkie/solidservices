namespace Client.Controllers
{
    using System;
    using Contract.Commands.Orders;
    using Contract.DTOs;

    public class CommandExampleController
    {
        private readonly ICommandHandler<CreateOrder> createOrderhandler;
        private readonly ICommandHandler<ShipOrder> shipOrderhandler;

        public CommandExampleController(
            ICommandHandler<CreateOrder> createOrderhandler,
            ICommandHandler<ShipOrder> shipOrderhandler)
        {
            this.createOrderhandler = createOrderhandler;
            this.shipOrderhandler = shipOrderhandler;
        }

        public Guid CreateOrder()
        {
            var createOrderCommand = new CreateOrder
            {
                NewOrderId = Guid.NewGuid(),
                ShippingAddress = new Address
                {
                    Country = "The Netherlands",
                    City = "Nijmegen",
                    Street = ".NET Street"
                }
            };

            this.createOrderhandler.Handle(createOrderCommand);

            Console.WriteLine("Order with ID {0} has been created.", createOrderCommand.NewOrderId);

            return createOrderCommand.NewOrderId;
        }

        public void ShipOrder(Guid orderId)
        {
            this.shipOrderhandler.Handle(new ShipOrder { OrderId = orderId });

            Console.WriteLine("Order with ID {0} is shipped.", orderId);
        }
    }
}