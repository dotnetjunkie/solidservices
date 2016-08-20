namespace Contract.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class Address
    {
        /// <summary>The country.</summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Country { get; set; }

        /// <summary>The city.</summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string City { get; set; }

        /// <summary>The street name including number.</summary>
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Street { get; set; }
    }
}