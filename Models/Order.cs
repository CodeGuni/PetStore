using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStore.Models
{
    public class Order
    {
        [Key] public int OrderID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required] public string Status { get; set; } = "Pending";

        // Clean & Simple Shipping Fields (exact match with Address table)
        [Required] public string ShippingStreetAddress { get; set; }
        [Required] public string ShippingCity { get; set; }
        [Required] public string ShippingProvince { get; set; }
        [Required] public string ShippingPostalCode { get; set; }
        public string ShippingLandmark { get; set; }

        // Optional: link to saved address
        public int? ShippingAddressID { get; set; }
        public Address? ShippingAddress { get; set; }

        public ICollection<OrderedItem> OrderedItems { get; set; } = new List<OrderedItem>();
    }
}