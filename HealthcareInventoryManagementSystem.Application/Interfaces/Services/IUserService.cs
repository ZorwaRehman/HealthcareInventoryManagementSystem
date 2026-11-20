using HealthcareInventoryManagementSystem.Application.DTOs.Users;
using HealthcareInventoryManagementSystem.Domain.Entities;

namespace HealthcareInventoryManagementSystem.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<IEnumerable<RoleEntity>> GetAllRolesAsync();

        Task AssignRoleAsync(
            int userId,
            int roleId,
            int requestingUserId);

        Task UpdateUserStatusAsync(
            int userId,
            bool isActive,
            int requestingUserId);
    }
}