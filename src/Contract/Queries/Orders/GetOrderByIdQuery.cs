namespace Contract.Queries.Orders
{
    using System;
    using Contract.DTOs;

    public class GetOrderByIdQuery : IQuery<OrderInfo>
    {
        public Guid OrderId { get; set; }
    }
}