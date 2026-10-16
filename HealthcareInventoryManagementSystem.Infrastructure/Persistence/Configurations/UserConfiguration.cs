
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthcareInventoryManagementSystem.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            // Set the database table name.
            builder.ToTable("Users");

            // Configure the primary key.
            builder.HasKey(user => user.Id);

            // Configure first name.
            builder.Property(user => user.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            // Configure last name.
            builder.Property(user => user.LastName)
                .IsRequired()
                .HasMaxLength(100);

            // Email must be unique.
            builder.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(user => user.Email)
                .IsUnique();

            // Store the password hash, never the plain-text password.
            builder.Property(user => user.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            // Configure the relationship between user and role.
            builder.HasOne(user => user.Role)
                .WithMany(role => role.Users)
                .HasForeignKey(user => user.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure account status.
            builder.Property(user => user.IsActive)
                .IsRequired();

            // Configure creation and update dates.
            builder.Property(user => user.CreatedAt)
                .IsRequired();

            builder.Property(user => user.UpdatedAt)
                .IsRequired(false);
        }
    }
}

