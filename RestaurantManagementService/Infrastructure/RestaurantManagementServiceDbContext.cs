using Microsoft.EntityFrameworkCore;
using RestaurantManagementService.Domain.Aggregates;
using RestaurantManagementService.Domain.ValueObjects;
using System;

namespace RestaurantManagementService.Infrastructure
{
    public class RestaurantManagementServiceDbContext : DbContext
    {
        public DbSet<RestaurantAggregate> Restaurants { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        public RestaurantManagementServiceDbContext(DbContextOptions<RestaurantManagementServiceDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure RestaurantAggregate
            modelBuilder.Entity<RestaurantAggregate>(entity =>
            {
                entity.HasKey(r => r.RestaurantId);

                // Map properties to database columns
                entity.Property(r => r.RestaurantId).HasColumnName("RestaurantId").IsRequired();
                entity.Property(r => r.Name).HasColumnName("Name").IsRequired();

                // Configure Address as an Owned Type (Value Object)
                entity.OwnsOne(r => r.Address, address =>
                {
                    address.Property(a => a.Street).HasColumnName("Street").IsRequired();
                    address.Property(a => a.City).HasColumnName("City").IsRequired();
                    address.Property(a => a.PostalCode).HasColumnName("PostalCode").IsRequired();
                });

                // Use parameterless constructor and bind explicitly
                entity.Ignore(r => r.MenuItems); // Ignore collection property during mapping
            });

            // Configure MenuItem
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Name).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Price).IsRequired().HasColumnType("decimal(18,2)");

                // Add a foreign key relationship with RestaurantAggregate
                entity.HasOne<RestaurantAggregate>()
                      .WithMany(r => r.MenuItems)
                      .HasForeignKey(m => m.RestaurantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }


    }
}
