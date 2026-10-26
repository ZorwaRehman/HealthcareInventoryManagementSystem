using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Application.DTOs.Alerts
{
    public class AlertDto
    {
        // General alert information
        public int AlertId { get; set; }

        public string AlertType { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Severity { get; set; } = string.Empty;

        // Related item information
        public int? ItemId { get; set; }

        public string? ItemName { get; set; }

        // Related stock batch information
        public int? StockBatchId { get; set; }

        public string? BatchNumber { get; set; }

        // Stock quantity information (mainly for low-stock alerts)
        public int? CurrentQuantity { get; set; }

        public int? ReorderLevel { get; set; }

        // Expiry information (mainly for expiry alerts)
        public DateTime? ExpiryDate { get; set; }

        // Alert tracking information
        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }
    }
}
