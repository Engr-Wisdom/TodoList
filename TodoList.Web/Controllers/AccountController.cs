using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace TodoList.Web.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // =========================================
    // REGISTER - GET
    // =========================================

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // =========================================
    // REGISTER - POST
    // =========================================

    [HttpPost]
    public async Task<IActionResult> Register(
        string firstName,
        string lastName,
        string email,
        string password,
        string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            ModelState.AddModelError(
                "firstName",
                "First name is required.");
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            ModelState.AddModelError(
                "lastName",
                "Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(
                "email",
                "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(
                "password",
                "Password is required.");
        }

        if (password != confirmPassword)
        {
            ModelState.AddModelError(
                "confirmPassword",
                "Passwords do not match.");
        }

        if (!ModelState.IsValid)
        {
            return View();
        }

        var client =
            _httpClientFactory.CreateClient("TodoApi");

        var request = new
        {
            firstName,
            lastName,
            email,
            password
        };

        var response =
            await client.PostAsJsonAsync(
                "api/auth/register",
                request);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content
                    .ReadFromJsonAsync<ApiError>();

            if (error?.Errors != null &&
                error.Errors.Count > 0)
            {
                foreach (var message in error.Errors)
                {
                    if (message.Contains(
                        "password",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(
                            "password",
                            message);
                    }
                    else if (message.Contains(
                        "email",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(
                            "email",
                            message);
                    }
                    else if (message.Contains(
                        "user name",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError(
                            "email",
                            message);
                    }
                    else
                    {
                        ModelState.AddModelError(
                            "",
                            message);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(
                    "email",
                    error?.Message ??
                    "Unable to create your account.");
            }

            return View();
        }

        TempData["SuccessMessage"] =
            "Account created successfully! Please log in to continue.";

        return RedirectToAction(nameof(Login));
    }

    // =========================================
    // LOGIN - GET
    // =========================================

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // =========================================
    // PROFILE - GET
    // =========================================

    [Authorize]
    [HttpGet("/account/profile")]
    public IActionResult Profile()
    {
        return View();
    }

    // =========================================
    // UPDATE PROFILE
    // =========================================

    [Authorize]
    [HttpPost("/account/profile/update")]
    public async Task<IActionResult> UpdateProfile(
        string firstName,
        string lastName,
        string email)
    {
        var userId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction(nameof(Login));
        }

        var client =
            _httpClientFactory.CreateClient("TodoApi");

        var request = new
        {
            firstName,
            lastName,
            email
        };

        var response =
            await client.PutAsJsonAsync(
                $"api/auth/profile/{userId}",
                request);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content
                    .ReadFromJsonAsync<ApiError>();

            ModelState.AddModelError(
                "",
                error?.Message ??
                "Unable to update your account.");

            return View("Profile");
        }

        var updatedUser =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        if (updatedUser == null)
        {
            ModelState.AddModelError(
                "",
                "Unable to update your account.");

            return View("Profile");
        }

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                updatedUser.UserId),

            new Claim(
                ClaimTypes.Name,
                $"{updatedUser.FirstName} {updatedUser.LastName}"),

            new Claim(
                ClaimTypes.Email,
                updatedUser.Email),

            new Claim(
                "FirstName",
                updatedUser.FirstName),

            new Claim(
                "LastName",
                updatedUser.LastName)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal =
            new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        TempData["SuccessMessage"] =
            "Your account has been updated successfully.";

        return Redirect("/account/profile");
    }

    // =========================================
    // LOGOUT
    // =========================================

    [Authorize]
    [HttpPost("/account/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

    // =========================================
    // DELETE ACCOUNT
    // =========================================

    [Authorize]
    [HttpPost("/account/delete")]
    public async Task<IActionResult> DeleteAccount()
    {
        var userId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction(nameof(Login));
        }

        var client =
            _httpClientFactory.CreateClient("TodoApi");

        var response =
            await client.DeleteAsync(
                $"api/auth/profile/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(
                "",
                "Unable to delete your account.");

            return View("Profile");
        }

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        TempData["SuccessMessage"] =
            "Your account has been deleted.";

        return RedirectToAction(nameof(Login));
    }

    // =========================================
    // LOGIN - POST
    // =========================================

    [HttpPost]
    public async Task<IActionResult> Login(
        string email,
        string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            ModelState.AddModelError(
                "email",
                "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError(
                "password",
                "Password is required.");
        }

        if (!ModelState.IsValid)
        {
            return View();
        }

        var client =
            _httpClientFactory.CreateClient("TodoApi");

        var request = new
        {
            email,
            password
        };

        var response =
            await client.PostAsJsonAsync(
                "api/auth/login",
                request);

        if (!response.IsSuccessStatusCode)
        {
            var error =
                await response.Content
                    .ReadFromJsonAsync<ApiError>();

            ModelState.AddModelError(
                "email",
                error?.Message ??
                "Invalid email or password.");

            return View();
        }

        var loginData =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();

        if (loginData == null)
        {
            ModelState.AddModelError(
                "email",
                "Unable to complete login.");

            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                loginData.UserId),

            new Claim(
                ClaimTypes.Name,
                $"{loginData.FirstName} {loginData.LastName}"),

            new Claim(
                ClaimTypes.Email,
                loginData.Email),

            new Claim(
                "FirstName",
                loginData.FirstName),

            new Claim(
                "LastName",
                loginData.LastName)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal =
            new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction(
            "Index",
            "Home");
    }
}

// =========================================
// API ERROR
// =========================================

public class ApiError
{
    public string Message { get; set; } = string.Empty;

    public List<string> Errors { get; set; } = new();
}

// =========================================
// LOGIN RESPONSE
// =========================================

public class LoginResponse
{
    public string UserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}