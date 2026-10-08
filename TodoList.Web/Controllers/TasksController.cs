using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TodoList.Web.Controllers;

[Authorize]
[Route("tasks")]
public class TasksController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TasksController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // =========================================
    // MY TASKS
    // =========================================

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var client = _httpClientFactory.CreateClient("TodoApi");

        var tasks =
            await client.GetFromJsonAsync<List<TaskViewModel>>(
                $"api/tasks?userId={Uri.EscapeDataString(userId)}");

        return View(
            "~/Views/Pages/MyTasks.cshtml",
            tasks ?? new List<TaskViewModel>());
    }

    // =========================================
    // ADD TASK - GET
    // =========================================

    [HttpGet("add")]
    public IActionResult Add()
    {
        return View("~/Views/Pages/AddTask.cshtml");
    }

    // =========================================
    // ADD TASK - POST
    // =========================================

    [HttpPost("add")]
    public async Task<IActionResult> Add(
        string title,
        string description,
        int category,
        int priority,
        DateOnly dueDate)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var client = _httpClientFactory.CreateClient("TodoApi");

        var task = new
        {
            userId = userId,
            title = title,
            summary = description,
            category = category,
            priority = priority,
            dueDate = dueDate,
            isCompleted = false
        };

        var response =
            await client.PostAsJsonAsync("api/tasks", task);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(
                "",
                "Unable to create the task. Please try again.");

            return View("~/Views/Pages/AddTask.cshtml");
        }

        return RedirectToAction(nameof(Index));
    }

    // =========================================
    // EDIT TASK - GET
    // =========================================

    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var client = _httpClientFactory.CreateClient("TodoApi");

        var task =
            await client.GetFromJsonAsync<TaskViewModel>(
                $"api/tasks/{id}?userId={Uri.EscapeDataString(userId)}");

        if (task == null)
        {
            return NotFound();
        }

        return View("~/Views/Pages/EditTask.cshtml", task);
    }

    // =========================================
    // EDIT TASK - POST
    // =========================================

    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(
        int id,
        string title,
        string description,
        int category,
        int priority,
        DateOnly dueDate,
        bool isCompleted)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var client = _httpClientFactory.CreateClient("TodoApi");

        var task = new
        {
            id = id,
            userId = userId,
            title = title,
            summary = description,
            category = category,
            priority = priority,
            dueDate = dueDate,
            isCompleted = isCompleted
        };

        var response =
            await client.PutAsJsonAsync(
                $"api/tasks/{id}",
                task);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(
                "",
                "Unable to update the task. Please try again.");

            var existingTask =
                await client.GetFromJsonAsync<TaskViewModel>(
                    $"api/tasks/{id}?userId={Uri.EscapeDataString(userId)}");

            return View(
                "~/Views/Pages/EditTask.cshtml",
                existingTask);
        }

        return RedirectToAction(nameof(Index));
    }

    // =========================================
    // DELETE TASK
    // =========================================

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var client = _httpClientFactory.CreateClient("TodoApi");

        var response =
            await client.DeleteAsync(
                $"api/tasks/{id}?userId={Uri.EscapeDataString(userId)}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}

public class TaskViewModel
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public int Category { get; set; }

    public DateOnly? DueDate { get; set; }

    public int Priority { get; set; }

    public bool IsCompleted { get; set; }
}