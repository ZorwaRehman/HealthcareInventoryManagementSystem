using HealthcareInventoryManagementSystem.Domain.Entities;
using HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ItemEntity> Items => Set<ItemEntity>();

        public DbSet<MedicineEntity> Medicines => Set<MedicineEntity>();

        public DbSet<StockBatchEntity> StockBatches => Set<StockBatchEntity>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<RoleEntity> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure a one-to-one relationship between Item and Medicine.
            modelBuilder.Entity<MedicineEntity>(entity =>
            {
                // Set MedicineId as the primary key.
                entity.HasKey(m => m.MedicineId);

                // Each medicine profile belongs to one item.
                // Each item can have at most one medicine profile.
                entity.HasOne(m => m.Item)
                    .WithOne()
                    .HasForeignKey<MedicineEntity>(m => m.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure StockBatch and Item relationship.
            modelBuilder.Entity<StockBatchEntity>(entity =>
            {
                // Set StockBatchId as the primary key.
                entity.HasKey(b => b.StockBatchId);

                // One item can have multiple stock batches.
                entity.HasOne(b => b.Item)
                    .WithMany()
                    .HasForeignKey(b => b.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Configure the batch number.
                entity.Property(b => b.BatchNumber)
                    .HasMaxLength(50)
                    .IsRequired();

                // Store unit price with two decimal places.
                entity.Property(b => b.UnitPrice)
                    .HasPrecision(18, 2);

                // Prevent duplicate batch numbers for the same item.
                entity.HasIndex(b => new { b.ItemId, b.BatchNumber })
                    .IsUnique();
            });

            // Configure User and Role entities.
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            // Configure Item entity.
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
        }
    }
}