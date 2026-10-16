using System;
using System.Collections.Generic;
using System.Text;

using HealthcareInventoryManagementSystem.Domain.Entities;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(UserEntity user, string roleName);

        DateTime GetTokenExpiration();
    }
}

