namespace Contract.Commands.Orders
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Contract.Validators;

    /// <summary>Commands an order to be shipped.</summary>
    public class ShipOrderCommand
    {
        /// <summary>The id of the order.</summary>
        [NonEmptyGuid]
        public Guid OrderId { get; set; }
    }
}