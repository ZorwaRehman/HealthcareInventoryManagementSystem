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

        // matching a condition.
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);

        // Determine whether a matching record exists.
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        // Add a new record.
        Task AddAsync(T entity);

        // UPDATE
        void Update(T entity);

        // Mark an existing record for deletion.
        void Delete(T entity);

        //  Save Changes
        Task SaveChangesAsync();
    }
}
