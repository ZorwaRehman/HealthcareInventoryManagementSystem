using System;
using System.Collections.Generic;
using System.Text;

using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace HealthcareInventoryManagementSystem.Infrastructure.Services
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<object> _passwordHasher;

        public PasswordHasherService()
        {
            _passwordHasher = new PasswordHasher<object>();
        }

        // Converts a plain-text password into a secure password hash.
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(new object(), password);
        }

        // Checks the entered password against its stored hash.
        public bool VerifyPassword(string password, string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                new object(),
                passwordHash,
                password);

            return result != PasswordVerificationResult.Failed;
        }
    }
}


