
using System.Security.Claims;
using HealthcareInventoryManagementSystem.Application.DTOs.Users;
using HealthcareInventoryManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareInventoryManagementSystem.Api.Controllers
{
    [Route("api/Users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        // Retrieve all registered users, including inactive accounts.
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            return Ok(users);
        }

        // GET: api/Users/roles
        // Retrieve all available roles.
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _userService.GetAllRolesAsync();

            return Ok(roles);
        }

        // PATCH: api/Users/{id}/role
        // Assign or change a user's role.
        [HttpPatch("{id:int}/role")]
        public async Task<IActionResult> AssignRole(
            int id,
            [FromBody] AssignRoleDto request)
        {
            try
            {
                if (request.RoleId <= 0)
                {
                    return BadRequest("Please select a valid role.");
                }

                var requestingUserId = GetRequestingUserId();

                if (requestingUserId == null)
                {
                    return Unauthorized();
                }

                await _userService.AssignRoleAsync(
                    id,
                    request.RoleId,
                    requestingUserId.Value);

                return Ok(new
                {
                    message = "User role updated successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PATCH: api/Users/{id}/status
        // Activate or deactivate a user's account.
        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateUserStatus(
            int id,
            [FromBody] UpdateUserStatusDto request)
        {
            try
            {
                var requestingUserId = GetRequestingUserId();

                if (requestingUserId == null)
                {
                    return Unauthorized();
                }

                await _userService.UpdateUserStatusAsync(
                    id,
                    request.IsActive,
                    requestingUserId.Value);

                return Ok(new
                {
                    message = request.IsActive
                        ? "User account activated successfully."
                        : "User account deactivated successfully."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // Retrieve the ID of the currently logged-in Admin.
        private int? GetRequestingUserId()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(userIdValue, out var userId)
                ? userId
                : null;
        }
    }
}
