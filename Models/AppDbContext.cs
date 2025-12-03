using Microsoft.EntityFrameworkCore;
using PetStore.Models;

namespace PetStore.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderedItem> OrderedItems { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fix the User ↔ Address relationship
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserID)
                .OnDelete(DeleteBehavior.SetNull);   // if user deleted, address stays (or use Cascade)

            // Fix DefaultAddress relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.DefaultAddress)
                .WithMany()
                .HasForeignKey(u => u.DefaultAddressID)
                .OnDelete(DeleteBehavior.SetNull);

            // Unique indexes
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}