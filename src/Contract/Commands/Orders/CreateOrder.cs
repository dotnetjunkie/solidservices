namespace Contract.Commands.Orders
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Contract.DTOs;
    using Contract.Validators;

    /// <summary>Creates a new order.</summary>
    public class CreateOrder : ICommand
    {
        /// <summary>The order id of the new order.</summary>
        [NonEmptyGuid]
        public Guid NewOrderId { get; set; }

        /// <summary>The order's shipping address.</summary>
        [Required, ValidateObject]
        public Address ShippingAddress { get; set; }
    }
}