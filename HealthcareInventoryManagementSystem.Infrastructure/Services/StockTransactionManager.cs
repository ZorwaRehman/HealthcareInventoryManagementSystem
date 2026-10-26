
using System;
using System.Threading.Tasks;
using HealthcareInventoryManagementSystem.Application.Interfaces;
using HealthcareInventoryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HealthcareInventoryManagementSystem.Infrastructure.Services
{
    // Manages database transactions for stock operations.
    public class StockTransactionManager(
        ApplicationDbContext context) : IStockTransactionManager
    {
        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            // Validate the operation before starting a transaction.
            ArgumentNullException.ThrowIfNull(operation);

            // Begin a database transaction.
            await using var transaction =
                await context.Database.BeginTransactionAsync();

            try
            {
                // Execute stock updates and add the transaction-history record.
                await operation();

                // Save all tracked changes using the same DbContext.
                await context.SaveChangesAsync();

                // Commit only after saving succeeds.
                await transaction.CommitAsync();
            }
            catch
            {
                // Roll back database changes if any operation fails.
                await transaction.RollbackAsync();

                // Preserve the original exception.
                throw;
            }
        }
    }
}
