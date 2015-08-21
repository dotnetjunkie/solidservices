namespace Contract.Commands.Orders
{
    using System;
    using System.Net;
    using Contract.DTOs;

    [WebApiResponse(HttpStatusCode.Created)]
    public class CreateOrderCommand
    {
        public Address ShippingAddress { get; set; }
        
        // Output property
        public Guid CreatedOrderId { get; set; }
    }
}