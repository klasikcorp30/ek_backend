using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ekklesia.Api.Models.DTOs;
using Ekklesia.Api.Services.Interfaces;
using Ekklesia.Api.Models;
using System.Security.Claims;

namespace Ekklesia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.LoginAsync(loginDto);
                
                if (result == null)
                    return Unauthorized("Invalid email or password");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", loginDto.Email);
                return StatusCode(500, "An error occurred during login");
            }
        }

        /// <summary>
        /// User registration
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (registerDto.Password != registerDto.ConfirmPassword)
                    return BadRequest("Password and confirm password do not match");

                var result = await _authService.RegisterAsync(registerDto);
                
                if (result == null)
                    return BadRequest("User with this email already exists");

                return CreatedAtAction(nameof(GetProfile), null, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", registerDto.Email);
                return StatusCode(500, "An error occurred during registration");
            }
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized();

                var user = await _authService.GetUserByIdAsync(userId);
                
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, "An error occurred while retrieving the profile");
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                    return Unauthorized();

                if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
                    return BadRequest("New password and confirm password do not match");

                var result = await _authService.ChangePasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                
                if (!result)
                    return BadRequest("Current password is incorrect");

                return Ok(new { Message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, "An error occurred while changing the password");
            }
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _authService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, "An error occurred while retrieving users");
            }
        }

        /// <summary>
        /// Update user role (Admin only)
        /// </summary>
        [HttpPatch("users/{userId}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUserRole(int userId, [FromBody] UpdateUserRoleDto updateRoleDto)
        {
            try
            {
                var result = await _authService.UpdateUserRoleAsync(userId, updateRoleDto.Role);
                
                if (!result)
                    return NotFound($"User with ID {userId} not found");

                return Ok(new { Message = "User role updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role for {UserId}", userId);
                return StatusCode(500, "An error occurred while updating the user role");
            }
        }

        /// <summary>
        /// Deactivate user (Admin only)
        /// </summary>
        [HttpDelete("users/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateUser(int userId)
        {
            try
            {
                var result = await _authService.DeactivateUserAsync(userId);
                
                if (!result)
                    return NotFound($"User with ID {userId} not found");

                return Ok(new { Message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user {UserId}", userId);
                return StatusCode(500, "An error occurred while deactivating the user");
            }
        }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class UpdateUserRoleDto
    {
        public UserRole Role { get; set; }
    }
}