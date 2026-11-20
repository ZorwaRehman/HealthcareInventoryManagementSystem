
using HealthcareInventoryManagementSystem.Application.DTOs.Users;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HealthcareInventoryManagementSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<UserEntity> _userRepository;
        private readonly IGenericRepository<RoleEntity> _roleRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IGenericRepository<UserEntity> userRepository,
            IGenericRepository<RoleEntity> roleRepository,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        // Retrieve all registered users, including inactive accounts.
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.FindAllAsync(
                user => true,
                user => user.Role!
            );

            return users.Select(user => new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }).ToList();
        }

        // Retrieve available roles.
        public async Task<IEnumerable<RoleEntity>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();

            return roles.Select(role => new RoleEntity
            {
                Id = role.Id,
                Name = role.Name
            }).ToList();
        }

        // Assign or change an existing user's role.
        public async Task AssignRoleAsync(
            int userId,
            int roleId,
            int requestingUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var selectedRole = await _roleRepository.GetByIdAsync(roleId);

            if (selectedRole == null)
            {
                throw new KeyNotFoundException("Selected role not found.");
            }

            // Retrieve the user's current role.
            var currentRole = await _roleRepository.GetByIdAsync(user.RoleId);

            if (currentRole == null)
            {
                throw new InvalidOperationException(
                    "The user's current role could not be found.");
            }

            bool isCurrentRoleAdmin =
                currentRole.Name.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);

            bool isSelectedRoleAdmin =
                selectedRole.Name.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);

            // Prevent an Admin from removing their own Admin access.
            if (userId == requestingUserId &&
                isCurrentRoleAdmin &&
                !isSelectedRoleAdmin)
            {
                throw new InvalidOperationException(
                    "You cannot change your own Admin role.");
            }

            // Prevent demoting the last active Admin.
            if (user.IsActive &&
                isCurrentRoleAdmin &&
                !isSelectedRoleAdmin)
            {
                var activeAdmins = await _userRepository.FindAllAsync(
                    existingUser =>
                        existingUser.IsActive &&
                        existingUser.RoleId == currentRole.Id
                );

                if (activeAdmins.Count() <= 1)
                {
                    throw new InvalidOperationException(
                        "The last active Admin cannot be demoted.");
                }
            }

            user.RoleId = roleId;
            user.Role = selectedRole;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);

            await _userRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Role {RoleName} assigned to user ID {UserId}.",
                selectedRole.Name,
                userId);
        }

        // Activate or deactivate an existing account.
        public async Task UpdateUserStatusAsync(
            int userId,
            bool isActive,
            int requestingUserId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // Prevent an Admin from deactivating their own account.
            if (userId == requestingUserId && !isActive)
            {
                throw new InvalidOperationException(
                    "You cannot deactivate your own account.");
            }

            // Prevent deactivation of the last active Admin.
            if (!isActive && user.IsActive)
            {
                var currentRole = await _roleRepository.GetByIdAsync(
                    user.RoleId);

                if (currentRole?.Name.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase) == true)
                {
                    var activeAdmins = await _userRepository.FindAllAsync(
                        existingUser =>
                            existingUser.IsActive &&
                            existingUser.RoleId == currentRole.Id
                    );

                    if (activeAdmins.Count() <= 1)
                    {
                        throw new InvalidOperationException(
                            "The last active Admin account cannot be deactivated.");
                    }
                }
            }

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);

            await _userRepository.SaveChangesAsync();

            _logger.LogInformation(
                "User ID {UserId} account status changed to {IsActive}.",
                userId,
                isActive);
        }
    }
}
