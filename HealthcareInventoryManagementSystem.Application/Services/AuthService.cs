
using HealthcareInventoryManagementSystem.Application.DTOs.Auth;
using HealthcareInventoryManagementSystem.Application.Interfaces.Repositories;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using HealthcareInventoryManagementSystem.Domain.Entities;

namespace HealthcareInventoryManagementSystem.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<UserEntity> _userRepository;
        private readonly IGenericRepository<RoleEntity> _roleRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IGenericRepository<UserEntity> userRepository,
            IGenericRepository<RoleEntity> roleRepository,
            IPasswordHasherService passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> RegisterAsync(
            RegisterRequestDto request)
        {
            // Check whether this email is already registered.
            var existingUser = await _userRepository.FindAsync(
                user => user.Email == request.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            // Find the default role.
            var defaultRole = await _roleRepository.FindAsync(
                role => role.Name == "User");

            if (defaultRole == null)
            {
                throw new InvalidOperationException(
                    "The default User role has not been configured.");
            }

            var user = new UserEntity
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                RoleId = defaultRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // The user can log in after the account is created.
            return new LoginResponseDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = defaultRole.Name
            };
        }

        public async Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request)
        {
            // Find the account using its email address.
            var user = await _userRepository.FindAsync(
                u => u.Email == request.Email.Trim());

            // Message for unknown users and wrong passwords.
            if (user == null ||
                !user.IsActive ||
                !_passwordHasher.VerifyPassword(
                    request.Password,
                    user.PasswordHash))
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }

            var role = await _roleRepository.GetByIdAsync(user.RoleId);

            if (role == null)
            {
                throw new InvalidOperationException(
                    "The user's assigned role could not be found.");
            }

            // Generate a JWT after successful authentication.
            var token = _tokenService.GenerateToken(user, role.Name);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = _tokenService.GetTokenExpiration(),
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = role.Name
            };
        }
    }
}
