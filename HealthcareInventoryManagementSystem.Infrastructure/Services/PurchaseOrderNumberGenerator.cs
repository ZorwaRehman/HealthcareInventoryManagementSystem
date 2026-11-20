using System;

using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Entities;
using HealthcareInventoryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;

namespace HealthcareInventoryManagementSystem.Infrastructure.Services
{
    public class PurchaseOrderNumberGenerator : IPurchaseOrderNumberGenerator
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderNumberGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateNextNumberAsync()
        {
            // Get the current Pakistan local date.
            var pakistanTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");

            DateTime pakistanDate =
                TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    pakistanTimeZone).Date;

            // Use a serializable transaction to prevent duplicate sequences.
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            // Lock the daily counter row (or its key range if it does not exist).
            var dailySequence = await _context.PurchaseOrderDailySequences
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM dbo.PurchaseOrderDailySequences
                    WITH (UPDLOCK, HOLDLOCK)
                    WHERE OrderDate = {pakistanDate}")
                .SingleOrDefaultAsync();

            int nextSequence;

            if (dailySequence == null)
            {
                // First Purchase Order for this Pakistan local date.
                dailySequence = new PurchaseOrderDailySequenceEntity
                {
                    OrderDate = pakistanDate,
                    LastSequence = 1
                };

                await _context.PurchaseOrderDailySequences.AddAsync(
                    dailySequence);

                nextSequence = 1;
            }
            else
            {
                // Increment the existing daily sequence.
                dailySequence.LastSequence++;

                nextSequence = dailySequence.LastSequence;
            }

            await _context.SaveChangesAsync();

            // Release the database lock after saving the new sequence.
            await transaction.CommitAsync();

            return $"PO-{pakistanDate:yyyyMMdd}-{nextSequence:D3}";
        }
    }
}
