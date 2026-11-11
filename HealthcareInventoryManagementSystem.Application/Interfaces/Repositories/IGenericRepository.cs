using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // Retrieve a record by its ID.
        Task<T?> GetByIdAsync(int id);

        // Retrieve all records.
        Task<IEnumerable<T>> GetAllAsync();

        // Find one record matching a condition.
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);

        // Retrieve records matching a condition.
        // Optional includes allow related entities to be loaded when needed.
        Task<IEnumerable<T>> FindAllAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        // Retrieve records with nested related entities.
        Task<IEnumerable<T>> FindAllWithNestedIncludesAsync(
            Expression<Func<T, bool>> predicate,
            params string[] includes);

        // Determine whether a matching record exists.
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        // Add a new record.
        Task AddAsync(T entity);

        // Update an existing record.
        void Update(T entity);

        // Mark an existing record for deletion.
        void Delete(T entity);

        // Save changes.
        Task SaveChangesAsync();
    }
}