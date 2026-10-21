using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace HealthcareInventoryManagementSystem.Application.Dtos
{
    public class ItemRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string Unit { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        [Url]
        public string? ImageUrl { get; set; }
    }
}

