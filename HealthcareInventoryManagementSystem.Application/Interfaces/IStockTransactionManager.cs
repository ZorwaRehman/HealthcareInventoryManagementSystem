using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Application.Interfaces
{
    // Defines the database transaction operation.
    public interface IStockTransactionManager
    {
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
