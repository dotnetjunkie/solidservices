namespace Contract.Commands.Orders
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Contract.DTOs;
    using Contract.Validators;

    public class CreateOrderCommand
    {
        [NonEmptyGuid]
        public Guid NewOrderId { get; set; }

        [Required, ValidateObject]
        public Address ShippingAddress { get; set; }
    }
}