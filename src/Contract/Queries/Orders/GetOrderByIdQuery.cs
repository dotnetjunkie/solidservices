namespace Contract.Queries.Orders
{
    using System;
    using Contract.DTOs;
    using Validators;

    /// <summary>Gets order information of a single order by its id.</summary>
    public class GetOrderByIdQuery : IQuery<OrderInfo>
    {
        /// <summary>The id of the order to get.</summary>
        [NonEmptyGuid]
        public Guid OrderId { get; set; }
    }
}