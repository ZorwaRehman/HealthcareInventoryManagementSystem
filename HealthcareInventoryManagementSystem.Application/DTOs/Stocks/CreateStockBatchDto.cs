using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace HealthcareInventoryManagementSystem.Application.DTOs.Stocks
{
    // Defines the data required to create a stock batch.
    public class CreateStockBatchDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string BatchNumber { get; set; } = string.Empty;

        public DateTime? ExpiryDate { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityInStock { get; set; }

        [Range(typeof(decimal), "0.01", "999999999")]
        public decimal UnitPrice { get; set; }
    }
}
