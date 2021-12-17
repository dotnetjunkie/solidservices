namespace Contract.Commands.Orders
{
    using System;
    using Contract.Validators;

    /// <summary>Commands an order to be shipped.</summary>
    public class ShipOrder : ICommand
    {
        /// <summary>The id of the order.</summary>
        [NonEmptyGuid]
        public Guid OrderId { get; set; }
    }
}