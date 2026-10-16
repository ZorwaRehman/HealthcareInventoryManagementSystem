using System;
using System.Collections.Generic;
using System.Text;
using HealthcareInventoryManagementSystem.Application.DTOs.Auth;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);

        bool VerifyPassword(string password, string passwordHash);
    }
}
