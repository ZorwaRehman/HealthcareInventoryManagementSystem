using System;
using System.Collections.Generic;
using System.Text;


namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface IPdfReportService
    {
        byte[] GeneratePdf<T>(
            IEnumerable<T> data,
            string reportTitle,
            string currencyCode = "PKR",
            decimal exchangeRate = 1m);
    }
}

