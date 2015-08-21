namespace BusinessLayer.QueryHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Contract;
    using Contract.DTOs;
    using Contract.Queries.Orders;

    public class GetUnshippedOrdersForCurrentCustomerQueryHandler
        : IQueryHandler<GetUnshippedOrdersForCurrentCustomerQuery, OrderInfo[]>
    {
        private readonly ILogger logger;

        public GetUnshippedOrdersForCurrentCustomerQueryHandler(ILogger logger)
        {
            this.logger = logger;
        }

        public OrderInfo[] Handle(GetUnshippedOrdersForCurrentCustomerQuery query)
        {
            this.logger.Log(string.Format("{0} {{ PageIndex = {1}, PageSize = {2} }}",
                query.GetType().Name, query.PageIndex, query.PageSize));

            return GetAllOrders()
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .ToArray();
        }

        private static IEnumerable<OrderInfo> GetAllOrders()
        {
            var random = new Random();

            return
                from number in Enumerable.Range(1, 100000)
                select new OrderInfo 
                { 
                    Id = Guid.NewGuid(), 
                    TotalAmount = random.Next(100, 1000), 
                    CreationDate = DateTime.Today.AddDays(-number)
                };
        }
    }
}