namespace Client
{
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            var orderController = Bootstrapper.GetCommandExampleController();

            var orderId = orderController.CreateOrder();

            orderController.ShipOrder(orderId);

            var showUnshippedOrdersController = Bootstrapper.GetQueryExampleController();

            showUnshippedOrdersController.ShowOrders(pageIndex: 0, pageSize: 10);

            Console.ReadLine();
        }
    }
}