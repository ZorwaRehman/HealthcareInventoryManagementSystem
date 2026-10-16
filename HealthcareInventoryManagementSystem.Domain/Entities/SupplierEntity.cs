using System;
using System.Collections.Generic;
using System.Text;

namespace HealthcareInventoryManagementSystem.Domain.Entities
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}
