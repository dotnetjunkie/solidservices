namespace Contract.Queries.Orders
{
    using Contract.DTOs;

    public class GetUnshippedOrdersForCurrentCustomerQuery : IQuery<Paged<OrderInfo>>
    {
        public PageInfo Paging { get; set; }
    }
}