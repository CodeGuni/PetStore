using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class User
    {
        [Key] public int UserID { get; set; }

        [Required, StringLength(50)] public string Username { get; set; }
        [Required] public string PasswordHash { get; set; }

        [Required, EmailAddress] public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        [Required] public string Role { get; set; } = "Customer";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Multiple proper addresses
        public ICollection<Address> Addresses { get; set; } = new List<Address>();

        // Optional default address
        public int? DefaultAddressID { get; set; }
        public Address? DefaultAddress { get; set; }

        // Navigation
        public ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}