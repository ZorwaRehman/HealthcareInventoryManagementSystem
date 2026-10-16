
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HealthcareInventoryManagementSystem.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T>
        where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // READ: Retrieve a record by its ID.
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        // READ: Retrieve all records.
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // READ: Find the first record matching a condition.
        public async Task<T?> FindAsync(
            Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        // CHECK: Check whether a matching record exists.
        public async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        // CREATE
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        // UPDATE
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        // DELETE
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        // SAVE
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

