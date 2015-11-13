namespace Contract.Commands.Orders
{
    using System;
    using Validators;

    public class ShipOrderCommand
    {
        [NonEmptyGuid]
        public Guid OrderId { get; set; }
    }
}