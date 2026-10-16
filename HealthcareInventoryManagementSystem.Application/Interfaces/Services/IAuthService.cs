using System;
using System.Collections.Generic;
using System.Text;
using HealthcareInventoryManagementSystem.Application.DTOs.Auth;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);
    }
}
