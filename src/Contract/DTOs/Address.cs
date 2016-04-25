namespace Contract.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class Address
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Country { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string City { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(100)]
        public string Street { get; set; }
    }
}