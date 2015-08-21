namespace Contract.Commands.Orders
{
    using System;

    [WebApiMethod(HttpMethod.Post)]
    public class ShipOrderCommand
    {
        public Guid OrderId { get; set; }
    }
}