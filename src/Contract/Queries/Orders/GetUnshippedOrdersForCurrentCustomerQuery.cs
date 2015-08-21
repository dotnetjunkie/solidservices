namespace Contract.Queries.Orders
{
    using Contract.DTOs;

    public class GetUnshippedOrdersForCurrentCustomerQuery : IQuery<OrderInfo[]>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}