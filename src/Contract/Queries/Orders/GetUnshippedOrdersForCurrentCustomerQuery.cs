namespace Contract.Queries.Orders
{
    using Contract.DTOs;

    /// <summary>
    /// Gets a paged list of all unshipped orders for the current logged in user.
    /// </summary>
    public class GetUnshippedOrdersForCurrentCustomerQuery : IQuery<Paged<OrderInfo>>
    {
        public PageInfo Paging { get; set; }
    }
}