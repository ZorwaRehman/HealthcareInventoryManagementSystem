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

        public DbSet<StockBatchEntity> StockBatches
            => Set<StockBatchEntity>();

        // Stores stock-in, stock-out, and adjustment history.
        public DbSet<StockTransactionEntity> StockTransactions
            => Set<StockTransactionEntity>();

        public DbSet<Supplier> Suppliers => Set<Supplier>();

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<RoleEntity> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the one-to-one relationship between Item and Medicine.
            modelBuilder.Entity<MedicineEntity>(entity =>
            {
                entity.HasKey(m => m.MedicineId);

                entity.HasOne(m => m.Item)
                    .WithOne()
                    .HasForeignKey<MedicineEntity>(m => m.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure StockBatchEntity and its relationship with ItemEntity.
            modelBuilder.ApplyConfiguration(
                new StockBatchConfiguration());

            // Configure StockTransactionEntity and its relationship with StockBatchEntity.
            modelBuilder.ApplyConfiguration(
                new StockTransactionConfiguration());

            // Configure User and Role entities.
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            // Configure Item entity.
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
        }
    }
}
