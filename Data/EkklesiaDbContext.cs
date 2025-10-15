using Microsoft.EntityFrameworkCore;
using Ekklesia.Api.Models;
using System.Text.Json;

namespace Ekklesia.Api.Data
{
    public class EkklesiaDbContext : DbContext
    {
        public EkklesiaDbContext(DbContextOptions<EkklesiaDbContext> options) : base(options)
        {
        }

        public DbSet<Church> Churches { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Church entity
            modelBuilder.Entity<Church>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(200);
                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.State).HasMaxLength(20);
                entity.Property(e => e.ZipCode).HasMaxLength(10);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Website).HasMaxLength(200);
                entity.Property(e => e.Denomination).HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedBy).HasMaxLength(100);
                entity.Property(e => e.UpdatedBy).HasMaxLength(100);
                
                // Index for better query performance
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.State);
                entity.HasIndex(e => e.Denomination);
                entity.HasIndex(e => e.IsActive);
                
                // Spatial index for location-based queries
                entity.HasIndex(e => new { e.Latitude, e.Longitude });
            });

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                
                // Unique constraint on email
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Seed initial admin user with static hash
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@ekklesia.com",
                    FirstName = "Admin",
                    LastName = "User",
                    PasswordHash = "$2a$11$Zo7z2H1YgW5FqC5qJ7Q9qu1f3sJ8xK4nL9qW7rE8pV2mN5tA6bC3.", // Static hash for Admin123!
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}