using System.ComponentModel.DataAnnotations;

namespace HealthcareInventoryManagementSystem.Application.DTOs.Stocks
{
    public class UpdateStockBatchNumberDto
    {
        [Required]
        [MaxLength(100)]
        public string BatchNumber { get; set; } = string.Empty;
    }
}