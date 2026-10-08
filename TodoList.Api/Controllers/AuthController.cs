using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Data;
using TodoList.Api.Models;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TodoContext _context;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        TodoContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    // =========================================
    // REGISTER
    // =========================================

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var existingUser =
            await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            return BadRequest(new
            {
                message = "An account with this email already exists."
            });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result =
            await _userManager.CreateAsync(
                user,
                request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Unable to create account.",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        return Ok(new
        {
            message = "Account created successfully."
        });
    }

    // =========================================
    // LOGIN
    // =========================================

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user =
            await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var passwordValid =
            await _userManager.CheckPasswordAsync(
                user,
                request.Password);

        if (!passwordValid)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        return Ok(new
        {
            message = "Login successful.",
            userId = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName
        });
    }

    // =========================================
    // UPDATE PROFILE
    // =========================================

    [HttpPut("profile/{userId}")]
    public async Task<IActionResult> UpdateProfile(
        string userId,
        UpdateProfileRequest request)
    {
        var user =
            await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User account not found."
            });
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            return BadRequest(new
            {
                message = "First name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            return BadRequest(new
            {
                message = "Last name is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(new
            {
                message = "Email is required."
            });
        }

        var existingUser =
            await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null &&
            existingUser.Id != userId)
        {
            return BadRequest(new
            {
                message = "An account with this email already exists."
            });
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = request.Email.Trim();
        user.UserName = request.Email.Trim();

        var result =
            await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Unable to update your account.",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        return Ok(new
        {
            message = "Account updated successfully.",
            userId = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName
        });
    }

    // =========================================
    // DELETE ACCOUNT
    // =========================================

    [HttpDelete("profile/{userId}")]
    public async Task<IActionResult> DeleteAccount(string userId)
    {
        var user =
            await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User account not found."
            });
        }

        // Delete all tasks belonging to this user first
        var userTasks =
            await _context.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();

        if (userTasks.Count > 0)
        {
            _context.Tasks.RemoveRange(userTasks);

            await _context.SaveChangesAsync();
        }

        // Now delete the user account
        var result =
            await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Unable to delete your account.",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        return Ok(new
        {
            message = "Account deleted successfully."
        });
    }
}

// =========================================
// REGISTER REQUEST
// =========================================

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

// =========================================
// LOGIN REQUEST
// =========================================

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

// =========================================
// UPDATE PROFILE REQUEST
// =========================================

public class UpdateProfileRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}