namespace Contract.DTOs
{
    using System;

    public class OrderInfo
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public decimal TotalAmount { get; set; }
    }
}