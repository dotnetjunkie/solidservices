namespace Contract.Queries.Orders
{
    using Contract.DTOs;

    /// <summary>
    /// Gets a paged list of all unshipped orders for the current logged in user.
    /// </summary>
    public class GetUnshippedOrders : IQuery<Paged<OrderInfo>>
    {
        /// <summary>The paging information.</summary>
        public PageInfo Paging { get; set; }
    }
}