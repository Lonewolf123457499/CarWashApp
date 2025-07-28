// using GreenWashBackendd
using Microsoft.EntityFrameworkCore;

namespace GreenWashBackend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Washer> Washers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<WashPackage> WashPackages { get; set; }
        public DbSet<Addon> Addons { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderAddon> OrderAddons { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureEntityRelationships(modelBuilder);

            // Seed an Admin user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName= "Admin User",
                Email = "admin@greenwash.com",
                Password = "admin123", // Use plain password as per your request
                Role = "Admin"
            });
        }

        private void ConfigureEntityRelationships(ModelBuilder modelBuilder)
        {
            // Customer ↔ User (1-to-1)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Washer ↔ User (1-to-1)
            modelBuilder.Entity<Washer>()
                .HasOne(w => w.User)
                .WithOne()
                .HasForeignKey<Washer>(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer ↔ Vehicle (1-to-many)
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Customer)
                .WithMany(c => c.Vehicles)
                .HasForeignKey(v => v.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer ↔ PaymentMethod (1-to-many)
            modelBuilder.Entity<PaymentMethod>()
                .HasOne(pm => pm.Customer)
                .WithMany(c => c.PaymentMethods)
                .HasForeignKey(pm => pm.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ FIXED HERE

            // Customer ↔ Order (1-to-many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Washer ↔ Order (1-to-many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Washer)
                .WithMany(w => w.Orders)
                .HasForeignKey(o => o.WasherId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Vehicle ↔ Order (1-to-many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Vehicle)
                .WithMany(v => v.Orders)
                .HasForeignKey(o => o.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // WashPackage ↔ Order (1-to-many)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.WashPackage)
                .WithMany(wp => wp.Orders)
                .HasForeignKey(o => o.WashPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order ↔ OrderAddon (many-to-many)
            modelBuilder.Entity<OrderAddon>()
                .HasKey(oa => new { oa.OrderId, oa.AddonId });

            modelBuilder.Entity<OrderAddon>()
                .HasOne(oa => oa.Order)
                .WithMany(o => o.OrderAddons)
                .HasForeignKey(oa => oa.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderAddon>()
                .HasOne(oa => oa.Addon)
                .WithMany(a => a.OrderAddons)
                .HasForeignKey(oa => oa.AddonId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order ↔ Rating (1-to-1)
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Order)
                .WithOne(o => o.Rating)
                .HasForeignKey<Rating>(r => r.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order ↔ Receipt (1-to-1)
            modelBuilder.Entity<Receipt>()
                .HasOne(rc => rc.Order)
                .WithOne(o => o.Receipt)
                .HasForeignKey<Receipt>(rc => rc.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
