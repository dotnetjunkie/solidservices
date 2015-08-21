namespace BusinessLayer.QueryHandlers
{
    using System;
    using System.Collections.Generic;
    using Contract;
    using Contract.DTOs;
    using Contract.Queries.Orders;

    public class GetOrderByIdQueryHandler : IQueryHandler<GetOrderByIdQuery, OrderInfo>
    {
        public OrderInfo Handle(GetOrderByIdQuery query)
        {
            if (query.OrderId == Guid.Empty)
            {
                throw new KeyNotFoundException();
            }

            return new OrderInfo
            {
                Id = query.OrderId,
                CreationDate = DateTime.Today,
                TotalAmount = 300
            };
        }
    }
}