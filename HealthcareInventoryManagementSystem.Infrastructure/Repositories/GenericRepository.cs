
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

        // READ: Retrieve records matching a condition.
        // Optional includes load related entities when needed.
        public async Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            // Start with the database query.
            IQueryable<T> query = _dbSet;

            // Load each requested related entity.
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply the condition and execute the query.
            return await query
                .Where(predicate)
                .ToListAsync();
        }


        // Retrieve records with nested related entities.
        public async Task<IEnumerable<T>> FindAllWithNestedIncludesAsync(
            Expression<Func<T, bool>> predicate,
            params string[] includes)
        {
            // Start with the database query.
            IQueryable<T> query = _dbSet;

            // Load related entities, including nested relationships.
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply the condition and execute the query.
            return await query
                .Where(predicate)
                .ToListAsync();
        }


        // Retrieve a page of records directly from the database.
        public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, object>> orderBy,
            int pageNumber,
            int pageSize,
            bool descending = false,
            params Expression<Func<T, object>>[] includes)
        {
            // Start with a read-only database query.
            IQueryable<T> query = _dbSet.AsNoTracking();

            // Apply filtering before counting and pagination.
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            // Count all matching records before pagination.
            int totalCount = await query.CountAsync();

            // Include related entities when requested.
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // Apply sorting.
            query = descending
                ? query.OrderByDescending(orderBy)
                : query.OrderBy(orderBy);

            // Retrieve only the requested page.
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
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