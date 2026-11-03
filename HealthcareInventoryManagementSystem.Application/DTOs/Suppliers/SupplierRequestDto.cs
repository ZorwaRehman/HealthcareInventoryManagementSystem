using System;
using System.Collections.Generic;
using System.Text;


namespace HealthcareInventoryManagementSystem.Application.DTOs.Suppliers
{
    // Contains supplier information received from API requests.
    public class SupplierRequestDto
    {
        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? ContactPerson { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? TaxNumber { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
