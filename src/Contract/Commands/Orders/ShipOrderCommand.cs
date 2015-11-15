namespace Contract.Commands.Orders
{
    using System;

    using Contract.Validators;

    public class ShipOrderCommand
    {
        [NonEmptyGuid]
        public Guid OrderId { get; set; }
    }
}