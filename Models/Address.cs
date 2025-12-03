using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class Address
    {
        [Key]
        public int AddressID { get; set; }

        [Required, StringLength(200)]
        public string StreetAddress { get; set; }     // House no., building, street name

        [Required, StringLength(100)]
        public string City { get; set; }

        [Required, StringLength(100)]
        public string Province { get; set; }          // or State

        [Required, StringLength(20)]
        public string PostalCode { get; set; }

        [Required, StringLength(100)]
        public string Country { get; set; } = "Canada";

        public string Landmark { get; set; }          // optional: "Near Jollibee"

        public string AddressLabel { get; set; }      // optional: "Home", "Office"

        // Links
        public int? UserID { get; set; }
        public User? User { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}